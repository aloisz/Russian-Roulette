using System;
using System.Collections;
using System.Collections.Generic;
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
    [Space]
    public List<Transform> playersPositions;

    [BoxGroup("Gun")] [Header("Gun")] public Gun gun;
    [BoxGroup("Gun")][SerializeField] private Bullet bullet;
    [BoxGroup("Gun")][SerializeField] private NetworkVariable<int> bulletNumber = new NetworkVariable<int>(0 , NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [BoxGroup("Gun")]public List<Bullet> presentedBullets = new List<Bullet>();

    [Space] [BoxGroup("Table")] public Table table;
    [Space] [BoxGroup("Objects")] public List<ObjectOnTable> objectOnTables;
    
    
    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        bulletNumber.OnValueChanged += (value, newValue) => bulletNumber.Value = newValue;
        
        if (!IsHost) return;
        StartCoroutine(ReloadGunCoroutine());        
        table.SpawnObjOnTable(3, 0);
        table.SpawnObjOnTable(2, 1);
    }

    [Rpc(SendTo.Server)]
    private void NextPlayerTurn_Rpc()
    {
        foreach (var player in PlayerControllers)
        {
            player.playerTurn.Value = !player.playerTurn.Value;
        }
    }

    [ContextMenu("Reload")]
    [Rpc(SendTo.Server)]
    public void ReloadGun_Rpc()
    {
        if (!IsHost) return;
        StartCoroutine(ReloadGunCoroutine());
    }

    
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
            int value = Random.Range(2, 3);

            bulletNumber.Value++;
            bullet.transform.name = bulletNumber.Value.ToString();
            bullet.transform.rotation *= Quaternion.Euler(Random.Range(-180,180), Random.Range(-180,180), Random.Range(-180,180));
            bullet.bulletID.Value = bulletNumber.Value;
            bullet.bulletType.Value = (BulletType)value;

            yield return new WaitForSeconds(.1f);
        }

        ShuffleBullet(presentedBullets);
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
    private void RemoveLife_Rpc(int targetClientID, int damage)
    {
        foreach (var player in PlayerControllers)
        {
            if (targetClientID == (int)player.OwnerClientId)
            {
                player.playerHealth.Value -= damage;
                Debug.Log($"___PlayerID {player.OwnerClientId} Health of value {player.playerHealth.Value}___");
            }
        }
    }
    
    [Rpc(SendTo.Server)]
    private void ShootBullet_Rpc(int targetClientID, int damage)
    {
        int lastBulletInChamber = 0;
        int randomBulletID = presentedBullets[lastBulletInChamber].bulletID.Value;
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
        bulletNumber.Value--;
    }

    /// <summary>
    /// Bullet was blank so you can still play 
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void StillYourTurn()
    {
        
    }

    public void RoundEnded(int targetClientID, int damage)
    {
        GameManager.Instance.ShootBullet_Rpc(targetClientID, damage);
        Debug.Log("Damage GameManager " + damage);
    }
}
