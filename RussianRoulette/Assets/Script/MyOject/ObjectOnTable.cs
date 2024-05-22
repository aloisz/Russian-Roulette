using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

public class ObjectOnTable : MyObject, IInteractOnContinue
{

    public NetworkVariable<int> clientListId = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> clientListIndex = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    
    public NetworkVariable<Vector3> basePos = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public  NetworkVariable<Vector3> offSet = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    [SerializeField]protected float timer = 0;
    protected float maxTimer = .1f;
    private bool canReturnToBasePos;
    [SerializeField]protected bool isObjectOnTableSelected;

    [Rpc(SendTo.Server)]
    public void SetClientInfo_Rpc(int id, int index)
    {
        if (id == 1) ChangeClient_Rpc();
        clientListId.Value = id;
        clientListIndex.Value = index;
    }
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        isObjectOnTableSelected = false;
        timer = 0;

        clientListId.OnValueChanged += (value, newValue) => clientListId.Value = newValue;
        clientListIndex.OnValueChanged += (value, newValue) => clientListIndex.Value = newValue;

        basePos.OnValueChanged += (value, newValue) => basePos.Value = newValue; 
        offSet.OnValueChanged += (value, newValue) => offSet.Value = newValue;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (clientListId.Value == 0) GameManager.Instance.table.tilesClient0[clientListIndex.Value].obj = null;
        else GameManager.Instance.table.tilesClient1[clientListIndex.Value].obj = null;
    }

    public void SetBasePos(Vector3 basePos)
    {
        this.basePos.Value = basePos;
        offSet.Value = this.basePos.Value + new Vector3(0, .1f, 0);
    }

    protected virtual void Update()
    {
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
            if(canReturnToBasePos) {transform.DOMove(basePos.Value, .5f);}
            canReturnToBasePos = false;
        }
    }

    [ContextMenu("Change")]
    [Rpc(SendTo.Server)]
    protected virtual void ChangeClient_Rpc()
    {
        NetworkObject.ChangeOwnership(1);
    }


    public virtual void InteractInContinue()
    {
        if(!IsOwner) return;
        timer = maxTimer;
        isObjectOnTableSelected = true;
        transform.DOMove(offSet.Value, .5f);
    }
}
