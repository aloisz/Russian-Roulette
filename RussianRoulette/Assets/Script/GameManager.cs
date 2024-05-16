using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GameManager : NetworkBehaviour
{
    public List<PlayerController> PlayerControllers;
    public CameraManager CameraManager;
    public PlayerHUD PlayerHUD;

    [Header("Gun")]     
    public Gun gun;

    [SerializeField] private Bullet bullet;
    [FormerlySerializedAs("bulletsInChamber")] public List<Bullet> presentedBullets = new List<Bullet>();
    
    [Space]
    public List<Transform> playersPositions;
    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }
    public override void OnNetworkDespawn()
    {       
       // bulletsInChamber?.Dispose();
       //NetworkObject.ChangeOwnership(0);
    }

    public override void OnNetworkSpawn()
    {
        //bulletsInChamber.OnListChanged += BulletsInChamberOnOnListChanged; 
        ReloadGun();
        foreach (var bullet in presentedBullets)
        {
            Debug.Log(bullet.name);
        }
    }

    private void BulletsInChamberOnOnListChanged(NetworkListEvent<int> changeevent)
    {
        //bulletsInChamber.Clear();
        Debug.Log(changeevent);
    }

    [Rpc(SendTo.Server)]
    public void NextPlayerTurn_Rpc()
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

        //StartCoroutine(wait());
    }
    
    /*private void ShuffleBullet(List<Bullet> bullets)
    {
        System.Random rng = new System.Random();
        int n = bullets.Count;
        while (n > 1)
        {   
            n--;
            int k = rng.Next(n + 1);
            (bullets[k], bullets[n]) = (bullets[n], bullets[k]);    
        }
    }*/

    /*private IEnumerator wait()
    {
        yield return new WaitForSeconds(2);
        ShuffleBullet(presentedBullets);
    }*/
    
    public void RoundEnded()
    {
        ReloadGun();
    }
}
