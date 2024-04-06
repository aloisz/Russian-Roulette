using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Player;
using UnityEngine;

public class Gun : MyObject
{

    private int count = 0;
    protected override void Select()
    {
        base.Select();
        transform.DOMove(GameManager.Instance.PlayerControllers[count].CameraManager.objPosition.position, .3f);
        transform.DORotate(Vector3.forward, .3f);
    }
    
    protected override void DeSelect()
    {
        base.DeSelect();
        transform.DOMove(basePosition, .3f);
        transform.rotation = baseRotation;
        count++;

    }
}
