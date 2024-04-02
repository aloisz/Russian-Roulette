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
    
    protected Vector3 basePosition;
    protected Quaternion baseRotation;
    
    public override void OnNetworkSpawn()
    {
        isSelected =  new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        isSelected.OnValueChanged += OnIsSelectedChanged;
        basePosition = transform.position;
        baseRotation = transform.rotation;
    }

    public override void OnNetworkDespawn()
    {
        isSelected.OnValueChanged -= OnIsSelectedChanged;
    }

    public virtual void OnIsSelectedChanged(bool previous, bool current)
    {
        Debug.Log("OnIsSelectedChanged");
        
        if (isSelected.Value)
        {
            //Selected Logic
        }
        else
        {
            //DeSelected Logic
        }
    }
    
    public void Interact(ulong OwnerClientId)
    {
        this.OwnedByClientId.Value = OwnerClientId; 
        isSelected.Value = !isSelected.Value;
    }
}

[System.Serializable]
public class HUD_OBJ
{
    public string actionName;
}

