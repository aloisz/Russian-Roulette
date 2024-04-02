using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using DG.Tweening;

public class MyObject : NetworkBehaviour, IInteractable
{
    [Header("Obj Info")]
    [SerializeField] protected NetworkVariable<bool> isSelected = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] protected NetworkVariable<ulong>  OwnedByClientId = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    
    public override void OnNetworkSpawn()
    {
        isSelected =  new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        isSelected.OnValueChanged += OnIsSelectedChanged;
    }

    public override void OnNetworkDespawn()
    {
        isSelected.OnValueChanged -= OnIsSelectedChanged;
    }

    public void OnIsSelectedChanged(bool previous, bool current)
    {
        Debug.Log("OnIsSelectedChanged");
        // note: `State.Value` will be equal to `current` here
        if (isSelected.Value)
        {
            transform.DOMove(new Vector3(0,1.1f,0), .3f);
            // door is open:
            //  - rotate door transform
            //  - play animations, sound etc.
        }
        else
        {
            transform.DOMove(Vector3.zero, .3f);
            // door is closed:
            //  - rotate door transform
            //  - play animations, sound etc.
        }
    }
    
    public void Interact(ulong OwnerClientId)
    {
        this.OwnedByClientId.Value = OwnerClientId; 
        isSelected.Value = true;
    }
    
}
