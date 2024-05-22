using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phone : ObjectOnTable
{
    protected override void Select(ulong OwnerClientId)
    {
        Debug.Log("Phone");
    }
}
