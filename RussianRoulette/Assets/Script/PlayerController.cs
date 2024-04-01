using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;
using UnityEditor;

namespace Player
{
    public class PlayerController : NetworkBehaviour
    {
        internal NetworkVariable<PlayerData> playerData = new NetworkVariable<PlayerData>(
            new PlayerData()
            {
                health = 100,
                stunt = false,
                message = "Hého !"
            }, 
            NetworkVariableReadPermission.Everyone, 
            NetworkVariableWritePermission.Owner);

        [Header("Player Config")] 
        [SerializeField] internal Transform cameraPos;
        
        public override void OnNetworkSpawn()
        {
            switch (OwnerClientId)
            {
                case 0:
                    transform.GetComponentInChildren<MeshRenderer>().material.color = Color.green;
                    transform.position = GameManager.Instance.playersPositions[0].position;
                    break;
                case 1:
                    transform.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
                    transform.position = GameManager.Instance.playersPositions[1].position;
                    transform.rotation *= Quaternion.Euler(0,180,0);
                    if(!IsOwner) return;
                    GameManager.Instance.SetCameraYAngle(180);
                    break;
            }
            
            if(!IsOwner) return;
            
            playerData.OnValueChanged += ((PlayerData previousValue, PlayerData newValue) =>
            {
                Debug.Log(OwnerClientId + " health " + newValue.health);
                Debug.Log(OwnerClientId + " Parle " + newValue.message);
            });
            GameManager.Instance.SetCameraTarget(cameraPos);
        }

        public void Update()
        {
            if(!IsOwner) return;
            if (Input.GetKeyDown(KeyCode.Space) && IsClient)
            {
                TestServerRpc(OwnerClientId);
            }
        }

        private void ShootRaycast(ulong sourceNetworkObjectId)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1000))
            {
                Debug.DrawRay(ray.origin, ray.direction * 1000, sourceNetworkObjectId == 0 ? Color.green : Color.red, 1);
                Debug.Log($"{hit.transform.name}     {sourceNetworkObjectId}");
            }
        }

        [Rpc(SendTo.Server)]
        void TestServerRpc(ulong sourceNetworkObjectId)
        {
            ShootRaycast(sourceNetworkObjectId);
            TestClientRpc(sourceNetworkObjectId); 
        }   
        
        [Rpc(SendTo.Everyone)]
        void TestClientRpc(ulong sourceNetworkObjectId)
        {
            ShootRaycast(sourceNetworkObjectId);
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

}
