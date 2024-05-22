using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adrenaline : ObjectOnTable
{
    protected override void Select(ulong OwnerClientId)
    {
        Debug.Log("Adrenaline");
    }
}
