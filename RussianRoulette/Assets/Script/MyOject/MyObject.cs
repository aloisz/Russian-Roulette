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
        /*PlayerHUD.Instance.EnableHUD(true);
        PlayerHUD.Instance.GetTheSelectedObj(this);
        PlayerHUD.Instance.DisplayBtns(true, HUD_OBJ);*/
        
        GameManager.Instance.PlayerControllers[(int)OwnedByClientId.Value].PlayerHUD.EnableHUD(true);
        GameManager.Instance.PlayerControllers[(int)OwnedByClientId.Value].PlayerHUD.GetTheSelectedObj(this);
        GameManager.Instance.PlayerControllers[(int)OwnedByClientId.Value].PlayerHUD.DisplayBtns(true, HUD_OBJ);
    }

    protected virtual void DeSelect()
    {
        GameManager.Instance.PlayerControllers[(int)OwnedByClientId.Value].PlayerHUD.DisplayBtns(false,null);
        GameManager.Instance.PlayerControllers[(int)OwnedByClientId.Value].PlayerHUD.EnableHUD(false);
       /* PlayerHUD.Instance.DisplayBtns(false, null);
        PlayerHUD.Instance.EnableHUD(false);*/
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
        TestClientRpc(OwnerClientId);
    }   
        
    [Rpc(SendTo.Everyone)]
    void TestClientRpc(ulong OwnerClientId)
    {
        ChangeIsSelectedValue();
        this.OwnedByClientId.Value = OwnerClientId;
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

