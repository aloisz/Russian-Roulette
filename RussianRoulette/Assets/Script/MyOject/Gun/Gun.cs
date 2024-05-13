using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Player;
using Unity.Netcode;
using UnityEngine;

public class Gun : MyObject
{
    [SerializeField] private Transform desiredPos;
    [SerializeField] private List<Bullet> bulletInChamber = new List<Bullet>();

    
    public void ReloadGun()
    {
        bulletInChamber.Clear();
        for (int i = 0; i < 5; i++)
        {
            Bullet bullet = Instantiate(GameManager.Instance.bullet, Vector3.zero, Quaternion.identity);
            int value = Random.Range(0, 2);
            bullet.bulletType = (BulletType)value;
            bulletInChamber.Add(bullet);
        }
    }

    public Bullet ShootBullet()
    {
        return bulletInChamber[^1];
    }
    
    protected override void Select(ulong OwnerClientId)
    {
        base.Select(OwnerClientId);
        transform.DOMove(GameManager.Instance.PlayerControllers[(int)OwnerClientId].CameraManager.objPosition
            .position, .3f);
        desiredPos = GameManager.Instance.PlayerControllers[(int)OwnerClientId].CameraManager.objPosition;

        transform.DORotate((int)OwnerClientId == 0 ? Vector3.forward : new Vector3(0, 180, 0), .3f);
    }
    
    protected override void DeSelect(ulong OwnerClientId)
    {
        base.DeSelect(OwnerClientId);
        transform.DOMove(basePosition, .3f);
        transform.rotation = baseRotation;
    }
}
