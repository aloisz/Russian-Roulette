using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class HealthScreen : NetworkBehaviour
{
    private TextMeshProUGUI[] healthClients;

    public static HealthScreen Instance;
    
    private void Awake()
    {
        Instance = this;
        
        healthClients = GetComponentsInChildren<TextMeshProUGUI>();
    }
    
    [Rpc(SendTo.Everyone)]
    public void SetClients_Rpc(int value, int cliendID)
    {
        healthClients[cliendID].text = value.ToString();
    }
}
