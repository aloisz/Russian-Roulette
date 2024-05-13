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
    public Bullet bullet;
    public NetworkVariable<List<Bullet>> bullets = new NetworkVariable<List<Bullet>>(new List<Bullet>(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    [Space]
    public List<Transform> playersPositions;
    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        ReloadGun();
    }

    [Rpc(SendTo.Server)]
    public void NextPlayerTurn_Rpc()
    {
        foreach (var player in PlayerControllers)
        {
            player.playerTurn.Value = !player.playerTurn.Value;
        }
    }

    
    private void ReloadGun()
    {
        if(!IsServer) return;
        bullets.Value.Clear();
        /*if (bullets.Value.Count != 0)
        {
            foreach (var bullet in bullets.Value)
            {
                bullet.GetComponent<NetworkObject>().Despawn();
            }
        }*/
        
        for (int i = 0; i < 5; i++)
        {
            //Bullet bullet = Instantiate(this.bullet, Vector3.zero, Quaternion.identity);
            //var bulletNetworkObject = bullet.GetComponent<NetworkObject>();
            //bulletNetworkObject.Spawn();
            int value = Random.Range(0, 2);
            bullet.bulletType = (BulletType)value;
            bullets.Value.Add(bullet);
            gun.AddIndex();
        }
    }
    
    public void RoundEnded()
    {
        ReloadGun();
    }
}
