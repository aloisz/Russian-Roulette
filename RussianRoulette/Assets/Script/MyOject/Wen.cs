using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wen : ObjectOnTable
{
    protected override void Select(ulong OwnerClientId)
    {
        Debug.Log("Wen");
        GameManager.Instance.PlayerControllers[(int)OwnerClientId].PlayerHUD.DisplayText("hehe", new Vector3(0,1.284f,0), 2f);
    }
}
