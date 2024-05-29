using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cigarette : ObjectOnTable
{
    [SerializeField] private int healthAdded = 2;
    protected override void Select(ulong OwnerClientId)
    {
        if(isStealing.Value) return;
        Debug.Log("Cigarette");
        GameManager.Instance.PlayerControllers[(int)OwnerClientId].AddHealth_Rpc(healthAdded);
    }
}
