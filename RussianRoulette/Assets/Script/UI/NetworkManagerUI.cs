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
    public TextMeshProUGUI partyJoinCode;
    public TMP_InputField joinCodeInput;

    [Space] 
    //[SerializeField] private Button hostSection;
    public Button joinSection;

    public static NetworkManagerUI Instance;

    private void Awake()
    {
        Instance = this;
        //btn_Host.onClick.AddListener((() => NetworkManager.Singleton.StartHost()));
        //btn_Server.onClick.AddListener((() => NetworkManager.Singleton.StartServer()));
        //btn_Client.onClick.AddListener((() => NetworkManager.Singleton.StartClient()));
    }
    
    private void Start()
    {
        btn_Client.onClick.AddListener(async () =>
        {
            if (RelayManager.Instance.IsRelayEnabled && !string.IsNullOrEmpty(joinCodeInput.text))
            {
                await RelayManager.Instance.JoinRelay(joinCodeInput.text);
                btn_Client.gameObject.SetActive(false);
                partyJoinCode.gameObject.SetActive(false);
                joinCodeInput.gameObject.SetActive(false);
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

            HostSection();
            NetworkManager.Singleton.StartHost();
        });
        joinSection.onClick.AddListener(JoinSection);
        
        btn_Client.gameObject.SetActive(false);
        partyJoinCode.gameObject.SetActive(false);
        joinCodeInput.gameObject.SetActive(false);
    }


    private void JoinSection()
    {
        btn_Host.gameObject.SetActive(false);
        joinSection.gameObject.SetActive(false);
        
        btn_Client.gameObject.SetActive(true);
        joinCodeInput.gameObject.SetActive(true);
    }

    private void HostSection()
    {
        btn_Host.gameObject.SetActive(false);
        joinSection.gameObject.SetActive(false);
        
        partyJoinCode.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (GameManager.Instance.PlayerControllers.Count < 0)  partyJoinCode.gameObject.SetActive(false);
    }
}