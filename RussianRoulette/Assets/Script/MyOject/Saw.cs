using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saw : ObjectOnTable
{
    [SerializeField] private int damageAdded = 1;
    protected override void Select(ulong OwnerClientId)
    {
        Debug.Log("Saw");
        GameManager.Instance.gun.AddDamage_Rpc(damageAdded);
    }
}
