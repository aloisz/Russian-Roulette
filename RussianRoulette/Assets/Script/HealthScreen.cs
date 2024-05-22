using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthScreen : MonoBehaviour
{
    private TextMeshProUGUI[] healthClients;

    public static HealthScreen Instance;
    
    private void Awake()
    {
        Instance = this;
        
        healthClients = GetComponentsInChildren<TextMeshProUGUI>();
    }

    public void SetClients(int value, int cliendID)
    {
        Debug.Log("ICIC");
        healthClients[cliendID].text = value.ToString();
    }
}
