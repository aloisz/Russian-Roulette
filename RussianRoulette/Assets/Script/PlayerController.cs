using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    private NetworkVariable<PlayerData> playerData = new NetworkVariable<PlayerData>(
        new PlayerData()
        {
            health = 100,
            stunt = false,
            message = "Hého !"
        }, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Owner);

    [Header("Player Config")] 
    public Animator animator;

    [HideInInspector]public Vector3 mouseDirection;

    
    public override void OnNetworkSpawn()
    {
        switch (OwnerClientId)
        {
            case 0:
                transform.GetComponentInChildren<MeshRenderer>().material.color = Color.green;
                break;
            case <= 1:
                transform.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
                break;
        }
        
        if(!IsOwner) return;
        playerData.OnValueChanged += ((PlayerData previousValue, PlayerData newValue) =>
        {
            Debug.Log(OwnerClientId + " health " + newValue.health);
            Debug.Log(OwnerClientId + " Parle " + newValue.message);
        });
        
        GameManager.Instance.SetCameraTarget(transform);
    }
}

public struct PlayerData : INetworkSerializable
{
    public int health;
    public bool stunt;
    public FixedString128Bytes message;
    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        //Serialize les Data qu'on veut
        serializer.SerializeValue(ref health);
        serializer.SerializeValue(ref stunt);
        serializer.SerializeValue(ref message);
    }
}
