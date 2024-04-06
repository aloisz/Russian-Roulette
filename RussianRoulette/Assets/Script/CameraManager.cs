using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private StateCamera StateCamera;
    
    [Header("Camera pos")]
    public Transform cameraPlayerPosition;

    [Header("Obj pos")] 
    public Transform objPosition; 
    
    [Header("Camera Setings")]
    [SerializeField] private Vector3 offSet;
    [SerializeField] private  float smoothTime = 2;
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
        
        switch (StateCamera)
        {
            case StateCamera.OnBeginPlay:
                break;
            case StateCamera.PlayerPos:
                camera.transform.position = 
                    Vector3.SmoothDamp(camera.transform.position, 
                        new Vector3(cameraPlayerPosition.transform.position.x, cameraPlayerPosition.transform.position.y, cameraPlayerPosition.transform.position.z) + offSet, 
                        ref currentVelocity, smoothTime);
                break;
        }
    }
}

public enum StateCamera
{
    OnBeginPlay,
    PlayerPos,
    
}
