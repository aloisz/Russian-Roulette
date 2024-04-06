using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public interface IInteractable
{
    public void Interact(ulong OwnerClientId);
}
