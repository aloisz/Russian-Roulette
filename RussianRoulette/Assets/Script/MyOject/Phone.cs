using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Phone : ObjectOnTable
{
    protected override void Select(ulong OwnerClientId)
    {
        Debug.Log("Phone");
        
        if(GameManager.Instance.presentedBullets.Count == 0) return;
        int randomBullet = Random.Range(0, GameManager.Instance.presentedBullets.Count);
        Bullet bulletIndex = GameManager.Instance.presentedBullets[randomBullet];

        int randomBullet2 = 0;
        while (randomBullet2 == randomBullet)
        {
            randomBullet2 = Random.Range(0, GameManager.Instance.presentedBullets.Count);
        }
        Bullet bulletIndex2 = GameManager.Instance.presentedBullets[randomBullet2];

        string displayText = String.Empty;
        if (bulletIndex.bulletType.Value == bulletIndex2.bulletType.Value)
        {
            displayText = $"2 Bullet {bulletIndex.bulletType.Value}";
        }
        else
        {
            displayText = $"1 Bullet {bulletIndex.bulletType.Value}, 1 Bullet {bulletIndex2.bulletType.Value}";
        }
        
        
        GameManager.Instance.PlayerControllers[(int)OwnerClientId].PlayerHUD.DisplayText(displayText, new Vector3(0,1.284f,0), 2f);
    }
}
