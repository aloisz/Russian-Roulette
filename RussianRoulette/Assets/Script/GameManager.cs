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

    [SerializeField] private Bullet bullet;
    [SerializeField] private NetworkVariable<int> bulletIndex = new NetworkVariable<int>(0 , NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
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
        bulletIndex.OnValueChanged += (value, newValue) => bulletIndex.Value = newValue;
        ReloadGun();
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
    private void ReloadGun()
    {
        if(!IsServer) return;
        for (int i = 0; i < 5; i++)
        {
            Bullet bullet = Instantiate(this.bullet, Vector3.zero, Quaternion.identity);
            var bulletNetworkObject = bullet.GetComponent<NetworkObject>();
            bulletNetworkObject.Spawn();
            int value = Random.Range(0, 2);
            bullet.transform.name = i.ToString();
            bullet.bulletType.Value = (BulletType)value;
        }

        bulletIndex.Value = presentedBullets.Count;
    }

    [Rpc(SendTo.Server)]
    private void RemoveLife_Rpc(int targetClientID, int damage)
    {
        foreach (var player in PlayerControllers)
        {
            if (targetClientID == (int)player.OwnerClientId)
            {
                player.playerHealth.Value -= damage;
                Debug.Log(player.playerHealth.Value);
            }
        }
    }
    
    [Rpc(SendTo.Server)]
    public void ShootBullet_Rpc(int targetClientID, int damage)
    {
        bulletIndex.Value--;
        RemoveLife_Rpc(targetClientID, damage);
    }
    
    public void RoundEnded()
    {
        NextPlayerTurn_Rpc();
    }
}
