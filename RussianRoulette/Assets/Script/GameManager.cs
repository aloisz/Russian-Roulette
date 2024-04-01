using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CameraManager CameraManager;
    public List<Transform> playersPositions;
    
    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }
    
    public void SetCameraTarget(Transform transform)
    {
        CameraManager.Player = transform;
    }

    public void SetCameraYAngle(float YAngle)
    {
        CameraManager.transform.rotation = Quaternion.Euler(0,YAngle, 0);
    }
}
