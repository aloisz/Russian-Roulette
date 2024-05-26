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
        if (OwnerClientId == 0)
        {
            foreach (var tiles in GameManager.Instance.table.tilesClient1)
            {
                if(tiles.obj != null) tiles.obj.ChangeClient_Rpc((int)OwnerClienID, true);
            }
        }
        else
        {
            foreach (var tiles in GameManager.Instance.table.tilesClient0)
            {
                if(tiles.obj != null) tiles.obj.ChangeClient_Rpc((int)OwnerClienID, true);
            }
        }
    }
}
