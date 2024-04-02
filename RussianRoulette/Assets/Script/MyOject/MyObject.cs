using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MyObject : NetworkBehaviour, IInteractable
{
    [Header("Obj Info")]
    [SerializeField] protected NetworkVariable<bool> isSelected;
    [SerializeField] protected NetworkVariable<ulong>  OwnedByClientId;
    
    public override void OnNetworkSpawn()
    {
        isSelected.OnValueChanged += OnIsSelectedChanged;
    }

    public override void OnNetworkDespawn()
    {
        isSelected.OnValueChanged -= OnIsSelectedChanged;
    }

    public void OnIsSelectedChanged(bool previous, bool current)
    {
        // note: `State.Value` will be equal to `current` here
        if (isSelected.Value)
        {
            // door is open:
            //  - rotate door transform
            //  - play animations, sound etc.
        }
        else
        {
            // door is closed:
            //  - rotate door transform
            //  - play animations, sound etc.
        }
    }
    
    public void Interact(ulong OwnerClientId)
    {
        this.OwnedByClientId = new NetworkVariable<ulong>(OwnerClientId);
        isSelected = new NetworkVariable<bool>(true);
    }
    
}
