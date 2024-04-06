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
        HUD.Instance.EnableHUD(true);
        HUD.Instance.GetTheSelectedObj(this);
        HUD.Instance.DisplayBtns(true, HUD_OBJ);
    }

    protected virtual void DeSelect()
    {
        HUD.Instance.DisplayBtns(false, null);
        HUD.Instance.EnableHUD(false);
    }
    
    public void Interact(ulong OwnerClientId)
    {
        /*this.OwnedByClientId.Value = OwnerClientId;
        ChangeIsSelectedValue();*/
        TestServerRpc(OwnerClientId);
    }
    
    [Rpc(SendTo.Server)]
    void TestServerRpc(ulong OwnerClientId)
    {
        this.OwnedByClientId.Value = OwnerClientId;
        ChangeIsSelectedValue();
        //TestClientRpc(OwnerClientId); 
    }   
        
    [Rpc(SendTo.Everyone)]
    void TestClientRpc(ulong OwnerClientId)
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
    public UnityEvent Event;
}

public enum ObjAction
{
    Normal,
    EndingRound
}

