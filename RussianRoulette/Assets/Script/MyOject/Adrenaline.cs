using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Adrenaline : ObjectOnTable
{
    protected override void Select(ulong OwnerClientId)
    {
        if(isStealing.Value) return;
        Debug.Log("Adrenaline");

        GameManager.Instance.PlayerControllers[(int)OwnerClientId].CameraManager.ChangeState(StateCamera.StealVision);
        DisplayInformation_Rpc(OwnerClientId);
    }

    [Rpc(SendTo.Server)]
    private void DisplayInformation_Rpc(ulong OwnerClienID)
    {
        foreach (var obj in GameManager.Instance.table.ObjectsOnTable)
        {
            if (OwnerClienID != obj.GetComponent<NetworkObject>().OwnerClientId)
            {
                obj.ChangeClient_Rpc((int)OwnerClienID, true);
            }
        }
    }
}
