using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    public Button btn_Host;
    public Button btn_Server;
    public Button btn_Client;

    
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private TextMeshProUGUI partyJoinCode;
    [SerializeField] private TMP_InputField joinCodeInput;

    /*private void Awake()
    {
        btn_Host.onClick.AddListener((() => NetworkManager.Singleton.StartHost()));
        //btn_Server.onClick.AddListener((() => NetworkManager.Singleton.StartServer()));
        btn_Client.onClick.AddListener((() => NetworkManager.Singleton.StartClient()));
    }*/
    
    private void Start()
    {
        btn_Client.onClick.AddListener(async () =>
        {
            if (RelayManager.Instance.IsRelayEnabled && !string.IsNullOrEmpty(joinCodeInput.text))
            {
                await RelayManager.Instance.JoinRelay(joinCodeInput.text);
            }

            NetworkManager.Singleton.StartClient();
        });
        btn_Host.onClick.AddListener(async () =>
        {
            if (RelayManager.Instance.IsRelayEnabled)
            {
                RelayHostData hostData = await RelayManager.Instance.SetupRelay();
                partyJoinCode.text = hostData.JoinCode;
            }

            NetworkManager.Singleton.StartHost();
        });
    }

    /*private void Update()
    {
        playerCountText.text = $"Player Count : {GameManager.Instance.playerCount.Value}";
    }*/
}