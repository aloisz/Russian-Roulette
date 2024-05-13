using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Player;
using Unity.Netcode;
using UnityEngine;

public class Gun : MyObject
{
    [SerializeField] private Transform desiredPos;
    //[SerializeField] private List<Bullet> bulletInChamber = new List<Bullet>();
    [SerializeField] private NetworkVariable<int> bulletIndex = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public int AddIndex()
    {
        return bulletIndex.Value++;
    }

    /*public void FillChamber(List<Bullet> bullets)
    {
        bulletInChamber.Clear();
        foreach (var bullet in bullets)
        {
            bulletInChamber.Add(bullet);
        }
    }*/

    public Bullet ShootBullet()
    {
        var lastBullet = GameManager.Instance.bullets.Value[bulletIndex.Value];
        GameManager.Instance.bullets.Value.RemoveAt(bulletIndex.Value);
        bulletIndex.Value--;
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
