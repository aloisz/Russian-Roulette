using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using DG.Tweening;
using Player;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class MyObject : NetworkBehaviour, IInteractable
{
    [Header("Obj Info")]
    [SerializeField] protected NetworkVariable<bool> isSelected = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] protected NetworkVariable<ulong>  OwnedByClientId = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    protected Vector3 basePosition;
    protected Quaternion baseRotation;

    [FormerlySerializedAs("ObjEffect")] [Header("HUD INFO")] 
    public ObjAction objAction;
    public List<HUD_OBJ> HUD_OBJ;
    
    public override void OnNetworkSpawn()
    {
        OwnedByClientId.OnValueChanged += (value, newValue) => OwnedByClientId.Value = newValue; 
        isSelected.OnValueChanged += OnIsSelectedChanged;
        
        basePosition = transform.position;
        baseRotation = transform.rotation;
    }

    public override void OnNetworkDespawn()
    {
        isSelected.OnValueChanged -= OnIsSelectedChanged;
    }
    
    private void OnIsSelectedChanged(bool previous, bool current)
    {
        //if(!IsOwner ) return;
        if (isSelected.Value)
        {
            Select(OwnedByClientId.Value);
        }
        else
        {
            DeSelect(OwnedByClientId.Value);
        }
    }

    protected virtual void Select(ulong OwnerClientId)
    {
        //if(!IsOwner) return;
        PlayerHUD.Instance.EnableHUD(true);
        PlayerHUD.Instance.SetPlayerId((int)OwnerClientId);
        PlayerHUD.Instance.GetTheSelectedObj(this);
        PlayerHUD.Instance.DisplayBtns(true, HUD_OBJ);
    }

    protected virtual void DeSelect(ulong OwnerClientId)
    {
        //if(!IsOwner) return;
        PlayerHUD.Instance.DisplayBtns(false, null);
        PlayerHUD.Instance.EnableHUD(false);
    }
    
    public void Interact(ulong OwnerClientId)
    {
        //if(!IsOwner) return;
        ClientId_Rpc(OwnerClientId);
    }
    
    [Rpc(SendTo.Server)]
    public void ChangeIsSelectedValue_Rpc()
    {
        isSelected.Value = !isSelected.Value;
    }
    
    [Rpc(SendTo.Server)]
    private void ClientId_Rpc(ulong OwnerClientId)
    {
        this.OwnedByClientId.Value = OwnerClientId;
        ChangeIsSelectedValue_Rpc();
    }
    
}

[System.Serializable]
public class HUD_OBJ
{
    public MyObject MyObject;
    public string actionName;
    public Transform transform;
    public UnityEvent Event;
}

public enum ObjAction
{
    Normal,
    EndingRound
}

