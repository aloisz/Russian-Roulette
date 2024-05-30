using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class ObjectOnTable : MyObject, IInteractOnContinue
{

    public NetworkVariable<int> clientListId = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> clientListIndex = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> isStealing = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    public NetworkVariable<Vector3> basePos = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public  NetworkVariable<Vector3> offSet = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    [SerializeField]protected float timer = 0;
    protected float maxTimer = .1f;
    private bool canReturnToBasePos;
    [SerializeField] protected bool isObjectOnTableSelected;
    protected Canvas _canvas;
    protected CanvasGroup _canvasGroup;
    protected TextMeshProUGUI[] _textActionName;
    private bool doDestroy = false;
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        isObjectOnTableSelected = false;
        timer = 0;
        _canvas = GetComponentInChildren<Canvas>();
        _canvasGroup = GetComponentInChildren<CanvasGroup>();
        _textActionName = GetComponentsInChildren<TextMeshProUGUI>();
        
        _canvasGroup.DOFade(0, 0);
        _textActionName[0].text = HUD_OBJ[0].actionName;
        _textActionName[1].text = HUD_OBJ[0].actionDescription;

        clientListId.OnValueChanged += (value, newValue) => clientListId.Value = newValue;
        clientListIndex.OnValueChanged += (value, newValue) => clientListIndex.Value = newValue;
        isStealing.OnValueChanged += (value, newValue) => isStealing.Value = newValue; 

        basePos.OnValueChanged += (value, newValue) => basePos.Value = newValue; 
        offSet.OnValueChanged += (value, newValue) => offSet.Value = newValue;

        GameManager.Instance.table.ObjectsOnTable.Add(this);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        GameManager.Instance.table.ObjectsOnTable.Remove(this);
    }
    
    [Rpc(SendTo.Server)]
    public void SetClientInfo_Rpc(int id, int index)
    {
        if (id == 1) ChangeClient_Rpc(1);
        clientListId.Value = id;
        clientListIndex.Value = index;
    }

    [Rpc(SendTo.Server)]
    public void SetBasePos_Rpc(Vector3 basePos)
    {
        this.basePos.Value = basePos;
        offSet.Value = this.basePos.Value + new Vector3(0, .1f, 0);
        SetBasePos_ClientRpc(basePos);
    }

    private bool doItOnce = false;
    [Rpc(SendTo.Everyone)]
    private void SetBasePos_ClientRpc(Vector3 basePos)
    {
        if(doItOnce) return;
        doItOnce = true;
        transform.position = basePos;
    }

    protected virtual void Update()
    {
        if(doDestroy) return;
        if(basePos == null) return;
        if(!ServerIsHost) return;
        if (isObjectOnTableSelected)
        {
            timer -= Time.deltaTime * 1;
            if (timer < 0)
            {
                canReturnToBasePos = true;
                isObjectOnTableSelected = false;
            }
        }
        else
        {
            if (canReturnToBasePos)
            {
                transform.DOMove(basePos.Value, .5f);
                _canvasGroup.DOFade(0, .5f);
            }
            canReturnToBasePos = false;
        }

        UpdateCanvasPosition();
    }
    
    private void UpdateCanvasPosition()//Update the orientation of the canvas
    {
        _canvas.transform.LookAt(_canvas.transform.position + Camera.main.transform.forward);
    }

    [ContextMenu("Change")]
    [Rpc(SendTo.Server)]
    public virtual void ChangeClient_Rpc(int newID)
    {
        NetworkObject.ChangeOwnership((ulong)newID);
    }

    
    [Rpc(SendTo.Server)]
    public virtual void ChangeClient_Rpc(int newID, bool isStealing)
    {
        NetworkObject.ChangeOwnership((ulong)newID);
        
        this.isStealing.Value = isStealing;
    }


    public override void Interact(ulong OwnerClientId)
    {
        if(OwnerClientId != NetworkObject.OwnerClientId) return;
        base.Interact(OwnerClientId);

        if (isStealing.Value)
        {
            IsStealing_Rpc((int)NetworkObject.OwnerClientId);
            CameraStealVision_ClientRpc((int)NetworkObject.OwnerClientId);
        }
        else
        {
            SetBasePos_Rpc(GameManager.Instance.PlayerControllers[(int)OwnerClientId].CameraManager.objPosition.position);
            StartCoroutine(DestroyCoroutine(OwnerClientId));
        }
    }
    
    private IEnumerator DestroyCoroutine(ulong OwnerClientId)
    {
        transform.GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(1.25f);
        DestroyObj_Rpc();
    }

    [Rpc(SendTo.Server)]
    private void DestroyObj_Rpc()
    {
        transform.GetComponent<NetworkObject>().Despawn();
    }
    
    [Rpc(SendTo.Everyone)]
    private void CameraStealVision_ClientRpc(int clientID)
    {
        GameManager.Instance.PlayerControllers[clientID].CameraManager.ChangeState(StateCamera.PlayerPos);
    }
    
    
    [Rpc(SendTo.Server)]
    private void IsStealing_Rpc(int cliendID)
    {
        if (cliendID == 0)
        {
            SetBasePos_Rpc(GameManager.Instance.table.tilesClient0[GameManager.Instance.table.tilesClient0Index.Value].transform.position);
            SetClientInfo_Rpc(cliendID, GameManager.Instance.table.tilesClient0Index.Value);
            isStealing.Value = false;
            isSelected.Value = false;
            GameManager.Instance.table.tilesClient0Index.Value++;
            
            foreach (var obj in GameManager.Instance.table.ObjectsOnTable)
            {
                if (obj.isStealing.Value)
                {
                    obj.ChangeClient_Rpc(1, false);
                }
            }
        }
        else
        {
            SetBasePos_Rpc(GameManager.Instance.table.tilesClient1[GameManager.Instance.table.tilesClient1Index.Value].transform.position);
            SetClientInfo_Rpc(cliendID, GameManager.Instance.table.tilesClient1Index.Value);
            isStealing.Value = false;
            isSelected.Value = false;
            GameManager.Instance.table.tilesClient1Index.Value++;
            
            foreach (var obj in GameManager.Instance.table.ObjectsOnTable)
            {
                if (obj.isStealing.Value)
                {
                    Debug.Log(obj, this);
                    obj.ChangeClient_Rpc(0, false);
                }
            }
        }
    }
    
    public virtual void InteractInContinue()
    {
        if(doDestroy) return;
        if(!IsOwner) return;
        timer = maxTimer;
        isObjectOnTableSelected = true;
        transform.DOMove(offSet.Value, .5f);
        _canvasGroup.DOFade(1, .1f);
    }
}
