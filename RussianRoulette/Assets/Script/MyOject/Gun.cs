using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Player;
using Unity.Netcode;
using UnityEngine;

public class Gun : MyObject
{
    [SerializeField] private Transform desiredPos;
    [SerializeField] private List<int> bulletInChamber;
    
    protected override void Select_Rpc(ulong OwnerClientId)
    {
        base.Select_Rpc(OwnerClientId);
        /*transform.position = GameManager.Instance.PlayerControllers[(int)OwnerClientId].CameraManager.objPosition
            .position;
        //transform.DOMove(, .3f);
        desiredPos = GameManager.Instance.PlayerControllers[(int)OwnerClientId].CameraManager.objPosition;
        transform.DORotate(Vector3.forward, .3f);*/
    }
    
    protected override void DeSelect_Rpc(ulong OwnerClientId)
    {
        base.DeSelect_Rpc(OwnerClientId);
        /*transform.DOMove(basePosition, .3f);
        transform.rotation = baseRotation;*/
    }
}
