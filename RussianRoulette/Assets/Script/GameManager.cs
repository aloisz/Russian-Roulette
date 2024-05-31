using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NaughtyAttributes;
using Player;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : NetworkBehaviour
{
    public List<PlayerController> PlayerControllers;
    public CameraManager CameraManager;
    public PlayerHUD PlayerHUD;
    [Range(1, 10)] [SerializeField] private int playersHealth;
    [Space]
    public List<Transform> playersPositions;

    [BoxGroup("Gun")] [Header("Gun")] public Gun gun;
    [BoxGroup("Gun")][SerializeField] private Bullet bullet;
    [BoxGroup("Gun")]public NetworkVariable<int> bulletNumber = new NetworkVariable<int>(0 , NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [BoxGroup("Gun")]public List<Bullet> presentedBullets = new List<Bullet>();

    [Space] [BoxGroup("Table")] public Table table;
    [Space] [BoxGroup("Objects")] public List<ObjectOnTable> objectOnTables;

    [Header("Transport")] 
    [SerializeField] private bool isRelay;

    public Action OnGameEnd;
    private NetworkVariable<bool> isGameFinished = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        bulletNumber.OnValueChanged += (value, newValue) => bulletNumber.Value = newValue;
        isGameFinished.OnValueChanged += (value, newValue) => isGameFinished.Value = newValue; 

        if (!IsHost) return;
        if(isRelay) WaitForClient();
        else
        {
            ClearTableAndReload_Rpc();
        }
    }
    
    private async void WaitForClient()
    {
        await WaitForAllPlayerToBeginGame();
        NetworkManagerUI.Instance.partyJoinCode.gameObject.SetActive(false);
        
        await Task.Delay(2000);
        
        ClearTableAndReload_Rpc();
        
        foreach (var player in PlayerControllers)
        {
            player.PlayerHUD.DisplayTextOnTV(player.playerTurn.Value == true ? "Your Turn" : "Wait", 2);
        }
    }
    
    async Task WaitForAllPlayerToBeginGame()
    {
        while (PlayerControllers.Count <= 1)
        {
            await Task.Yield();
        }
    }


    // -----------------------
    #region TURN LOGIC

    private int countTillNewItems = 0;

    [Rpc(SendTo.Server)]
    private void NextPlayerTurn_Rpc()
    {
        foreach (var player in PlayerControllers)
        {
            player.playerTurn.Value = !player.playerTurn.Value;
        }
        
        
        if(!IsHost) return;
        
        //if no more obj on the table refill it 
        countTillNewItems++;
        if (countTillNewItems % 2 == 0)
        {
            table.SpawnObjOnTable(1, 0);
            table.SpawnObjOnTable(1, 1);
        }
        
        if(table.ObjectsOnTable.Count == 0) StartCoroutine(ClearTable());
        if (bulletNumber.Value == 0) ReloadGun_Rpc();
        
        // orientate light toward player turn
        Light.Instance.RotateLightToPlayerTurn_Rpc(PlayerControllers[0].OwnerClientId == 0 ? 0 : 1);
    }
    
    
    
    /// <summary>
    /// Bullet was blank so you can still play 
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void StillYourTurn()
    {
        if(bulletNumber.Value == 0) ReloadGun_Rpc();
    }

    public void RoundEnded(int targetClientID, int damage)
    {   
        GameManager.Instance.ShootBullet_Rpc(targetClientID, damage);
        Debug.Log("Damage GameManager " + damage);
    }

    private void GameFinished()
    {
        OnGameEnd?.Invoke();
        if(!IsHost) return;
        isGameFinished.Value = false;
    }

    #endregion
    
    // -----------------------
    #region GUN

    
    [ContextMenu("Reload")]
    [Rpc(SendTo.Server)]
    public void ReloadGun_Rpc()
    {
        if (!IsHost) return;
        StartCoroutine(ReloadGunCoroutine());
    }

    private bool doOnce = false;
    private IEnumerator ReloadGunCoroutine()
    {
        if (bulletNumber.Value != 0)
        {
            var bullets = GameObject.FindGameObjectsWithTag("Bullet");
            foreach (var bullet in bullets)
            {
                bullet.GetComponent<NetworkObject>().Despawn();
            }
        }
        
        yield return new WaitForSeconds(1f);
        bulletNumber.Value = 0;
        for (int i = 0; i < 5; i++)
        {
            Bullet bullet = Instantiate(this.bullet, Vector3.up * 2, Quaternion.identity);
            var bulletNetworkObject = bullet.GetComponent<NetworkObject>();
            bulletNetworkObject.Spawn();
            int value = Random.Range(1, 3);

            bulletNumber.Value++;
            bullet.transform.name = bulletNumber.Value.ToString();
            bullet.transform.rotation *= Quaternion.Euler(Random.Range(-180,180), Random.Range(-180,180), Random.Range(-180,180));
            bullet.bulletID.Value = bulletNumber.Value;
            bullet.bulletType.Value = (BulletType)value;

            yield return new WaitForSeconds(.1f);
        }

        //ShuffleBullet(presentedBullets);
        if (doOnce) yield break;
        doOnce = true;
        Light.Instance.RotateLightToPlayerTurn_Rpc(0);
    }
    
    private void ShuffleBullet(List<Bullet> bullets)
    {
        System.Random rng = new System.Random();
        int n = bullets.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (bullets[k], bullets[n]) = (bullets[n], bullets[k]);
        }
    }
    
    [Rpc(SendTo.Server)]
    private void ShootBullet_Rpc(int targetClientID, int damage)
    {
        bulletNumber.Value--;
        
        int lastBulletInChamber = 0;
        int randomBulletID = presentedBullets[0].bulletID.Value;
        BulletType randomBulletValue = presentedBullets[lastBulletInChamber].bulletType.Value;
        
        if (randomBulletValue == BulletType.Live)
        {
            Debug.Log($"<color=blue>Shoot bulletID {randomBulletID} of value {randomBulletValue}</color>");
            RemoveLife_Rpc(targetClientID, damage);
            NextPlayerTurn_Rpc();
        }
        else
        {
            Debug.Log($"<color=blue>Shoot bulletID {randomBulletID} of value {randomBulletValue}</color>");
            
            if (targetClientID == (int)gun.OwnerClientId)
            {
                StillYourTurn();
            }
            else NextPlayerTurn_Rpc();
        }

        gun.ResetDamage();
        presentedBullets[lastBulletInChamber].GetComponent<NetworkObject>().Despawn();
    }
    

    #endregion
    
    // -----------------------

    #region TABLE

    [ContextMenu("Clear Table")]
    [Rpc(SendTo.Server)]
    public void ClearTableAndReload_Rpc()
    {
        StartCoroutine(ClearTable());
        StartCoroutine(ReloadGunCoroutine());
        foreach (var player in PlayerControllers)
        {
            player.playerHealth.Value = playersHealth;
        }
    }

    private IEnumerator ClearTable()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Object");
        foreach (var objectOnTable in objects)
        {
            objectOnTable.GetComponent<NetworkObject>().Despawn();
            table.tilesClient0Index.Value = 0;
            table.tilesClient1Index.Value = 0;
        }
        yield return new WaitForSeconds(1);
        table.SpawnObjOnTable(3, 0);
        table.SpawnObjOnTable(3, 1);
    }

    #endregion
    // -----------------------
    #region Health

    [Rpc(SendTo.Server)]
    private void RemoveLife_Rpc(int targetClientID, int damage)
    {
        foreach (var player in PlayerControllers)
        {
            if (targetClientID == (int)player.OwnerClientId)
            {
                player.playerHealth.Value -= damage;
                Debug.Log($"___PlayerID {player.OwnerClientId} Health of value {player.playerHealth.Value}___");
            }
            
            if (player.playerHealth.Value <= 0)isGameFinished.Value = true;

            RemoveLife_ClientRpc((int)player.OwnerClientId);

            if (player.playerHealth.Value <= 0)
            {
                ClearTableAndReload_Rpc();
            }
        }
    }

    [Rpc(SendTo.Everyone)]
    private void RemoveLife_ClientRpc(int clientID)
    {
        if (isGameFinished.Value)
        {
            GameFinished();
        }
        
        StartCoroutine(CameraHealthMonitor(PlayerControllers[clientID], 1.25f));
    }

    private IEnumerator CameraHealthMonitor(PlayerController player , float elapsedTime)
    {
        yield return new WaitForSeconds(elapsedTime);
        player.CameraManager.ChangeState(StateCamera.HealthMonitor);
        yield return new WaitForSeconds(elapsedTime);
        player.CameraManager.ChangeState(StateCamera.PlayerPos);

        if (!IsHost) yield break;
        if(bulletNumber.Value == 0) ReloadGun_Rpc();
    }

    #endregion

    

    
}
