using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Transform> playersPositions;
    
    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }
    
    
}
