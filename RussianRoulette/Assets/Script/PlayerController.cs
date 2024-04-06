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
            NetworkVariableWritePermission.Server);
        
        [SerializeField] internal NetworkVariable<int> playerHealth = new NetworkVariable<int>(7,NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);

        [Header("Player Config")] 
        public MyObject objInHand;
        public CameraManager CameraManager;
        [SerializeField] internal Transform cameraPos;
        
        public override void OnNetworkSpawn()
        {
            GameManager.Instance.PlayerControllers.Add(this);
            CameraManager = GameManager.Instance.CameraManager;
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
            playerTurn.OnValueChanged += OnPlayerTurnChanged;
            switch (OwnerClientId)
            {
                case 0:
                    playerTurn.Value = true; 
                    break;
                case 1:
                    playerTurn.Value = false; 
                    transform.rotation *= Quaternion.Euler(0,180,0);
                    CameraManager.SetCameraYAngle(new Vector3(0,180,0));
                    break;
            }
            
            CameraManager.SetCameraTarget(CameraManager.cameraPlayerPosition);
        }
        
        public override void OnNetworkDespawn()
        {
            playerTurn.OnValueChanged -= OnPlayerTurnChanged;;
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
        
        public void OnPlayerTurnChanged(bool previous, bool current)
        {
            Debug.Log($"Player turn {playerTurn.Value}");
            if (playerTurn.Value)
            {
                
            }
            else
            {
                
            }
        }

        [Rpc(SendTo.Server)]
        void TestServerRpc(ulong sourceNetworkObjectId)
        {
            TestClientRpc(sourceNetworkObjectId); 
        }   
        
        [Rpc(SendTo.Everyone)]
        void TestClientRpc(ulong sourceNetworkObjectId)
        {
            ShootRaycast(sourceNetworkObjectId);
        }
        
    }
}
