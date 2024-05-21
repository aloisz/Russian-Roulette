using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : NetworkBehaviour
{
    public List<PlayerController> PlayerControllers;
    public CameraManager CameraManager;
    public PlayerHUD PlayerHUD;

    [Header("Gun")]     
    public Gun gun;
    //[SerializeField] private NetworkVariable<int> clientIDGunPosses = new NetworkVariable<int>(0 , NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] private Bullet bullet;
    [SerializeField] private NetworkVariable<int> bulletNumber = new NetworkVariable<int>(0 , NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public List<Bullet> presentedBullets = new List<Bullet>();
    
    [Space]
    public List<Transform> playersPositions;
    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        bulletNumber.OnValueChanged += (value, newValue) => bulletNumber.Value = newValue;
        //clientIDGunPosses.OnValueChanged += (value, newValue) => clientIDGunPosses.Value = newValue;
        
        if (!IsHost) return;
        StartCoroutine(ReloadGun());        
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
    private void StartCoroutine()
    {
        if (!IsHost) return;
        StartCoroutine(ReloadGun());
    }

    
    private IEnumerator ReloadGun()
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
        int randomBullet = Random.Range(0, bulletNumber.Value - 1);
        int randomBulletID = presentedBullets[randomBullet].bulletID.Value;
        BulletType randomBulletValue = presentedBullets[randomBullet].bulletType.Value;

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

        presentedBullets[randomBullet].GetComponent<NetworkObject>().Despawn();
        bulletNumber.Value--;
    }

    /// <summary>
    /// Bullet was blank so you can still play 
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void StillYourTurn()
    {
        //throw new NotImplementedException();
    }

    public void RoundEnded(int targetClientID, int damage)
    {
        GameManager.Instance.ShootBullet_Rpc(targetClientID, damage);
    }
}
