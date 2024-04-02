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
    
    [Header("HUD INFO")] 
    public List<HUD_OBJ> HUD_OBJ;
    
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

    public void OnIsSelectedChanged(bool previous, bool current)
    {
        
        if (isSelected.Value)
        {
            //Selected Logic
            Select();
        }
        else
        {
            //DeSelected Logic
            DeSelect();
        }
    }

    protected virtual void Select()
    {
        HUD.Instance.GetTheSelectedObj(this);
        HUD.Instance.DisplayBtns(true, HUD_OBJ);
    }

    protected virtual void DeSelect()
    {
        HUD.Instance.DisplayBtns(false, null);
    }
    
    public void Interact(ulong OwnerClientId)
    {
        this.OwnedByClientId.Value = OwnerClientId;
        ChangeIsSelectedValue();
    }

    public void ChangeIsSelectedValue()
    {
        isSelected.Value = !isSelected.Value;
    }
}

[System.Serializable]
public class HUD_OBJ
{
    public MyObject MyObject;
    public string actionName;
    public Transform transform;
}

