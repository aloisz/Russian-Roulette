using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scissors : ObjectOnTable
{
    protected override void Select(ulong OwnerClientId)
    {
        if(isStealing.Value) return;
        Debug.Log("Scissors");
        
        if(GameManager.Instance.presentedBullets.Count == 0) return;

        BulletType actualBulletType = GameManager.Instance.presentedBullets[0].bulletType.Value;

        switch (actualBulletType)
        {
            case BulletType.NUll:
                break;
            case BulletType.Blank:
                GameManager.Instance.presentedBullets[0].ChangeBulletValue_Rpc(BulletType.Live);
                break;
            case BulletType.Live:
                GameManager.Instance.presentedBullets[0].ChangeBulletValue_Rpc(BulletType.Blank);
                break;
        }
    }
}
