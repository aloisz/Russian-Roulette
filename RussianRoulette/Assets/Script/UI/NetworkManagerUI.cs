using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    public Button btn_Host;
    public Button btn_Server;
    public Button btn_Client;


    private void Awake()
    {
        btn_Host.onClick.AddListener((() => NetworkManager.Singleton.StartHost()));
        //btn_Server.onClick.AddListener((() => NetworkManager.Singleton.StartServer()));
        btn_Client.onClick.AddListener((() => NetworkManager.Singleton.StartClient()));
    }
}