using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Liquor : ObjectOnTable
{
    protected override void Select(ulong OwnerClientId)
    {
        Debug.Log("Liquor");
    }
}
