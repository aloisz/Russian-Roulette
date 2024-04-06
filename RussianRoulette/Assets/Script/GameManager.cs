using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public List<PlayerController> PlayerControllers;
    public CameraManager CameraManager;
    
    public List<Transform> playersPositions;
    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    [Rpc(SendTo.Server)]
    public void RoundEndedRpc()
    {
        RoundEndedClientRpc();
    }
    
    [Rpc(SendTo.Everyone)]
    private void RoundEndedClientRpc()
    {
        if(!IsOwner) return;
        RoundEnded();
    }

    private void RoundEnded()
    {
        foreach (var player in PlayerControllers)
        {
            player.playerTurn.Value = !player.playerTurn.Value;
        }
    }
}
