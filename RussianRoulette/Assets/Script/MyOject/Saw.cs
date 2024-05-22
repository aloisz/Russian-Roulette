using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saw : ObjectOnTable
{
    protected override void Select(ulong OwnerClientId)
    {
        Debug.Log("Saw");
    }
}
