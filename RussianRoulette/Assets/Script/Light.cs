using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

public class Light : NetworkBehaviour
{
    public static Light Instance;
    private Vector3 baseRot;
    
    private void Awake()
    {
        Instance = this;
        baseRot = transform.rotation.eulerAngles;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    [Rpc(SendTo.Everyone)]
    public void RotateLightToPlayerTurn_Rpc(int cliendID)
    {
        
        if (cliendID == 0)
        {
            transform.DOComplete();
            transform.DORotate(
                new Vector3(104, transform.rotation.eulerAngles.y,
                    transform.rotation.eulerAngles.z), 1.25f);
        }
        else
        {
            Debug.Log("ROTATE LIGHT");
            transform.DOComplete();
            transform.DORotate(
                new Vector3(76, transform.rotation.eulerAngles.y,
                    transform.rotation.eulerAngles.z), 1.25f);
        }
        
    }
}
