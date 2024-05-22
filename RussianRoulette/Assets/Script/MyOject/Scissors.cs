using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scissors : ObjectOnTable
{
    protected override void Select(ulong OwnerClientId)
    {
        Debug.Log("Scissors");
    }
}
