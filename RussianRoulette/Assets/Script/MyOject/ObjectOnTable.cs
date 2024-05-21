using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

public class ObjectOnTable : MyObject, IInteractOnContinue
{
    protected Vector3 basePos;
    protected Vector3 offSet;
    [SerializeField]protected float timer = 0;
    protected float maxTimer = .1f;
    [SerializeField]protected bool isObjectOnTableSelected;
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        basePos = transform.position;
        offSet = basePos + new Vector3(0, .1f, 0);

        isObjectOnTableSelected = false;
        timer = 0;
    }

    protected virtual void Update()
    {
        if(!ServerIsHost) return;
        if (isObjectOnTableSelected)
        {
            timer -= Time.deltaTime * 1;
            if (timer < 0)
            {
                isObjectOnTableSelected = false;
            }
        }
        else transform.DOMove(basePos, .5f);
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
        transform.DOMove(offSet, .5f);
    }
}
