using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Phone : ObjectOnTable
{
    protected override void Select(ulong OwnerClientId)
    {
        if(isStealing.Value) return;
        Debug.Log("Phone");
        
        switch (GameManager.Instance.bulletNumber.Value)
        {
            case 0:
                return;
            case 1:
                GameManager.Instance.PlayerControllers[(int)OwnerClientId].PlayerHUD.DisplayText(lastBullet(), new Vector3(0,1.284f,0), 2f);
                break;
            default:
                GameManager.Instance.PlayerControllers[(int)OwnerClientId].PlayerHUD.DisplayText(BulletsLeft(), new Vector3(0,1.284f,0), 2f);
                break;
        }
    }


    private string lastBullet()
    {
        Bullet bulletIndex = GameManager.Instance.presentedBullets[0];
        string displayText = String.Empty;
        displayText = $"1 Bullet {bulletIndex.bulletType.Value}";
        return displayText; 
    }
    
    private string BulletsLeft()
    {
        int randomBullet = Random.Range(0, GameManager.Instance.presentedBullets.Count);
        Bullet bulletIndex = GameManager.Instance.presentedBullets[randomBullet];

        int randomBullet2 = 0;
        while (randomBullet2 == randomBullet)
        {
            randomBullet2 = Random.Range(0, GameManager.Instance.presentedBullets.Count);
        }
        Bullet bulletIndex2 = GameManager.Instance.presentedBullets[randomBullet2];

        string displayText = String.Empty;
        displayText = bulletIndex.bulletType.Value 
                      == bulletIndex2.bulletType.Value 
            ? $"2 Bullets {bulletIndex.bulletType.Value}" 
            : $"1 Bullet {bulletIndex.bulletType.Value}, 1 Bullet {bulletIndex2.bulletType.Value}";

        return displayText;
    }
}
