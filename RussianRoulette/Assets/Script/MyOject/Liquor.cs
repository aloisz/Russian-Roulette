using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Liquor : ObjectOnTable
{
    protected override void Select(ulong OwnerClientId)
    {
        if(isStealing.Value) return;
        Debug.Log("Liquor");
        GameManager.Instance.ReloadGun_Rpc();
    }
}
