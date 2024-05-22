using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cigarette : ObjectOnTable
{
    protected override void Select(ulong OwnerClientId)
    {
        Debug.Log("Cigarette");
    }
}
