using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Player;
using Unity.Netcode;
using UnityEngine;

public class Gun : MyObject
{
    [SerializeField] private Transform desiredPos;
    [SerializeField] private int bulletIndex;

    public int AddIndex()
    {
        return bulletIndex++;
    }

    public Bullet ShootBullet()
    {
        var lastBullet = GameManager.Instance.presentedBullets[bulletIndex];
        GameManager.Instance.presentedBullets.RemoveAt(bulletIndex);
        bulletIndex--;
        return lastBullet;
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
