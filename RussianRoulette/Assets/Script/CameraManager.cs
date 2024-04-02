using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera camera;
    public Transform Player;
    
    [Header("Camera Setings")]
    [SerializeField] private Vector3 offSet;
    [SerializeField] private  float smoothTime = 2;
    private Vector3 currentVelocity;
    
    
    public static CameraManager Instance;

    private void Awake()
    {
        Instance = this;
    }
    
    public void SetCameraTarget(Transform transform)
    {
        Player = transform;
    }

    public void SetCameraYAngle(Vector3 CameraAngle)
    {
        camera.transform.rotation = Quaternion.Euler(CameraAngle);
    }
    
    public void LateUpdate()
    {
        if(Player == null) return;
        
        camera.transform.position = 
            Vector3.SmoothDamp(camera.transform.position, 
                new Vector3(Player.transform.position.x, Player.transform.position.y, Player.transform.position.z) + offSet, 
                ref currentVelocity, smoothTime);
    }
}
