using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class HUD : MonoBehaviour
{
    public List<Button> texts;
    
    public static HUD Instance;
    private void Awake()
    {
        Instance = this;
    }
    
    
}
