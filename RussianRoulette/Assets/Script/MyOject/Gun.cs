using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Player;
using UnityEngine;

public class Gun : MyObject
{
    [SerializeField] private Transform desiredPos;
    protected override void Select(ulong OwnerClientId)
    {
        base.Select(OwnerClientId);
        /*transform.DOMove(GameManager.Instance.PlayerControllers[(int)OwnerClientId].CameraManager.objPosition.position, .3f);
        desiredPos = GameManager.Instance.PlayerControllers[(int)OwnerClientId].CameraManager.objPosition;*/
        transform.DORotate(Vector3.forward, .3f);
    }
    
    protected override void DeSelect(ulong OwnerClientId)
    {
        base.DeSelect(OwnerClientId);
        transform.DOMove(basePosition, .3f);
        transform.rotation = baseRotation;
    }
}
