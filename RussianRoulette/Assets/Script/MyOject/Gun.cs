using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Gun : MyObject
{

    
    

    protected override void Select()
    {
        base.Select();
        transform.DOMove(CameraManager.Instance.objPosition.transform.position, .3f);
        transform.DORotate(Vector3.forward, .3f);
        
        
    }
    
    protected override void DeSelect()
    {
        base.DeSelect();
        transform.DOMove(basePosition, .3f);
        transform.rotation = baseRotation;
        
    }
}
