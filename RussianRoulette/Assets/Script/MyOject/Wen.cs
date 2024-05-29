using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wen : ObjectOnTable
{
    protected override void Select(ulong OwnerClientId)
    {
        if(isStealing.Value) return;
        Debug.Log("Wen");

        if(GameManager.Instance.presentedBullets.Count == 0) return;
        Bullet bulletIndex = GameManager.Instance.presentedBullets[0];
        string displayText = $"Bullet {bulletIndex.bulletType.Value}";
        
        GameManager.Instance.PlayerControllers[(int)OwnerClientId].PlayerHUD.DisplayText(displayText, new Vector3(0,1.284f,0), 2f);
    }
}
