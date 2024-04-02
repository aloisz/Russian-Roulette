using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Gun : MyObject
{

    [Header("HUD INFO")] 
    public List<HUD_OBJ> HUD_OBJ;
    
    public override void OnIsSelectedChanged(bool previous, bool current)
    {
        Debug.Log("OnIsSelectedChanged");
        
        if (isSelected.Value)
        {
            transform.DOMove(CameraManager.Instance.objPosition.transform.position, .3f);
            transform.DORotate(Vector3.forward, .3f);
        }
        else
        {
            transform.DOMove(basePosition, .3f);
            transform.rotation = baseRotation;
        }
    }
}
