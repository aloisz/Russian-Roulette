using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;
using UnityEditor;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] internal NetworkVariable<bool> playerTurn = new NetworkVariable<bool>(false,NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);
        
        [SerializeField] internal NetworkVariable<int> playerHealth = new NetworkVariable<int>(7,NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);

        [Header("Player Config")] 
        public MyObject objInHand;
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
                    break;
            }
            
            if(!IsOwner) return;
            
            switch (OwnerClientId)
            {
                case 0:
                    playerTurn = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone,
                        NetworkVariableWritePermission.Owner);
                    break;
                case 1:
                    playerTurn = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone,
                        NetworkVariableWritePermission.Owner);
                    transform.rotation *= Quaternion.Euler(0,180,0);
                    CameraManager.Instance.SetCameraYAngle(new Vector3(0,180,0));
                    break;
            }
            
            Debug.Log($"Players turn is {playerTurn.Value}");
            
            CameraManager.Instance.SetCameraTarget(CameraManager.Instance.cameraPlayerPosition);
        }

        public void Update()
        {
            if(!IsOwner) return;
            if (Input.GetKeyDown(KeyCode.Mouse0) && IsClient && playerTurn.Value)
            {
                ShootRaycast(OwnerClientId);
            }
        }

        private void ShootRaycast(ulong OwnerClientId)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1000))
            {
                Debug.DrawRay(ray.origin, ray.direction * 1000, OwnerClientId == 0 ? Color.green : Color.red, 1);
                
                if (hit.transform.GetComponent<IInteractable>() != null)
                {
                    hit.transform.GetComponent<IInteractable>().Interact(OwnerClientId);
                    //Debug.Log($"Touch Object : {hit.transform.name}  OwnerClientId :{OwnerClientId}");
                }
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
}
