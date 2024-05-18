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
    
    protected Vector3 basePosition;
    protected Quaternion baseRotation;

    [FormerlySerializedAs("ObjEffect")] [Header("HUD INFO")] 
    public ObjAction objAction;
    public List<HUD_OBJ> HUD_OBJ;
    
    public override void OnNetworkSpawn()
    {
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
        if(!IsOwner) return;
        if (isSelected.Value)
        {
            Select(NetworkObject.OwnerClientId);
        }
        else
        {
            DeSelect(NetworkObject.OwnerClientId);
        }
    }
    
    protected virtual void Select(ulong OwnerClientId)
    {
        GameManager.Instance.PlayerControllers[(int)OwnerClientId].PlayerHUD.GetTheSelectedObj(this);
        GameManager.Instance.PlayerControllers[(int)OwnerClientId].PlayerHUD.DisplayBtns(true, HUD_OBJ);
    }
    
    protected virtual void DeSelect(ulong OwnerClientId)
    {
        GameManager.Instance.PlayerControllers[(int)OwnerClientId].PlayerHUD.DisplayBtns(false, null);
    }
    
    public void Interact(ulong OwnerClientId)
    {
        ClientId_Rpc(OwnerClientId);
        ChangeIsSelectedValue_Rpc();
    }
    
    [Rpc(SendTo.Server)]
    public void ChangeIsSelectedValue_Rpc()
    { 
        isSelected.Value = !isSelected.Value;
    }
    
    [Rpc(SendTo.Server)]
    private void ClientId_Rpc(ulong OwnerClientId)
    { 
        NetworkObject.ChangeOwnership(OwnerClientId);
    }
    
}

[System.Serializable]
public class HUD_OBJ
{
    public MyObject MyObject;
    public string actionName;
    public List<HUD_OBJ_Spec> HUDObjSpecs;
    public UnityEvent Event;
}

[System.Serializable]
public class HUD_OBJ_Spec
{
    public Transform Transforms;
    public int targetClientID;
}

public enum ObjAction
{
    Normal,
    EndingRound
}

