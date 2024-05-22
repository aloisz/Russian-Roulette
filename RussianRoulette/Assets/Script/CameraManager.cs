using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraManager : MonoBehaviour
{
    public int clientID;
    [SerializeField] private Camera camera;
    [SerializeField] private StateCamera StateCamera;
    
    [Header("Camera pos")]
    public Transform cameraPlayerPosition;
    [SerializeField] private Transform tableVision;
    [SerializeField] private List<Transform> healthTransform;

    [Header("Obj pos")] 
    public Transform objPosition; 
    
    [Header("Camera Setings")]
    [SerializeField] private Vector3 offSet;
    [SerializeField] private  float smoothTime = 2;
    [SerializeField] private  float rotInterpolation = 2;
    private Vector3 currentVelocity;
    
    private void Awake()
    {
        StateCamera = StateCamera.OnBeginPlay;
    }

    public StateCamera ChangeState(StateCamera stateCamera)
    {
        return this.StateCamera = stateCamera;
    }
    
    public void SetCameraTarget(Transform transform)
    {
        cameraPlayerPosition = transform;
        ChangeState(StateCamera.PlayerPos);
    }

    public void SetCameraYAngle(Vector3 CameraAngle)
    {
        transform.rotation = Quaternion.Euler(CameraAngle);
    }
    
    public void LateUpdate()
    {
        if(cameraPlayerPosition == null) return;
        CheckInput();
        
        switch (StateCamera)
        {
            case StateCamera.OnBeginPlay:
                break;
            case StateCamera.PlayerPos:
                camera.transform.position = 
                    Vector3.SmoothDamp(camera.transform.position, 
                        new Vector3(cameraPlayerPosition.transform.position.x, cameraPlayerPosition.transform.position.y, cameraPlayerPosition.transform.position.z) + offSet, 
                        ref currentVelocity, smoothTime);
                
                camera.transform.rotation = Quaternion.Slerp(camera.transform.rotation, cameraPlayerPosition.rotation, Time.deltaTime * rotInterpolation);
                break;
            case StateCamera.TableVision:
                TableVision();
                break;
            case StateCamera.HealthMonitor:
                HealthMonitor();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            ChangeState(StateCamera.TableVision);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            ChangeState(StateCamera.PlayerPos);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            ChangeState(StateCamera.HealthMonitor);
        }
    }


    private void TableVision()
    {
        camera.transform.position = 
            Vector3.SmoothDamp(camera.transform.position, 
                new Vector3(tableVision.position.x, tableVision.position.y, tableVision.position.z) + offSet, 
                ref currentVelocity, smoothTime);
        
        camera.transform.rotation = Quaternion.Slerp(camera.transform.rotation, tableVision.rotation, Time.deltaTime * rotInterpolation);
    }

    private void HealthMonitor()
    {
        camera.transform.position = 
            Vector3.SmoothDamp(camera.transform.position, 
                new Vector3(healthTransform[clientID].position.x, healthTransform[clientID].position.y, healthTransform[clientID].position.z) + offSet, 
                ref currentVelocity, smoothTime);
    
        camera.transform.rotation = Quaternion.Slerp(camera.transform.rotation, healthTransform[clientID].rotation, Time.deltaTime * rotInterpolation);
    }
}

public enum StateCamera
{
    OnBeginPlay,
    PlayerPos,
    TableVision,
    HealthMonitor
}
