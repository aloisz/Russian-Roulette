using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform Player;
    [Header("Camera Setings")]
    [SerializeField] private Vector3 offSet;
    [SerializeField] private  float smoothTime = 2;
    private Vector3 currentVelocity;
    
    public void LateUpdate()
    {
        if(Player == null) return;
        
        transform.position = 
            Vector3.SmoothDamp(transform.position, new Vector3(Player.transform.position.x, 0, Player.transform.position.z) + offSet, ref currentVelocity, smoothTime);
    }
}
