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
    public Button joinSection;
    public Button quitButton;

    public Button ruleButton;
    public Button backToMenu;
    public RectTransform ruleTxt;
    public TextMeshProUGUI credit;

    public static NetworkManagerUI Instance;

    private void Awake()
    {
        Instance = this;
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

        ruleButton.onClick.AddListener(RuleSection);
        ruleTxt.gameObject.SetActive(false);
        backToMenu.onClick.AddListener(BackToMenu);
        backToMenu.gameObject.SetActive(false);
        
        quitButton.onClick.AddListener((() => Application.Quit()));
        quitButton.gameObject.SetActive(false);
        
        
        btn_Client.gameObject.SetActive(false);
        partyJoinCode.gameObject.SetActive(false);
        joinCodeInput.gameObject.SetActive(false);
        
        ruleButton.gameObject.SetActive(true);
        btn_Host.gameObject.SetActive(true);
        joinSection.gameObject.SetActive(true);
        credit.gameObject.SetActive(true);
    }


    private void JoinSection()
    {
        ruleButton.gameObject.SetActive(false);
        btn_Host.gameObject.SetActive(false);
        joinSection.gameObject.SetActive(false);
        
        btn_Client.gameObject.SetActive(true);
        joinCodeInput.gameObject.SetActive(true);
        credit.gameObject.SetActive(false);
    }

    private void HostSection()
    {
        ruleButton.gameObject.SetActive(false);
        btn_Host.gameObject.SetActive(false);
        joinSection.gameObject.SetActive(false);
        
        partyJoinCode.gameObject.SetActive(true);
        credit.gameObject.SetActive(false);
    }

    private void RuleSection()
    {
        ruleButton.gameObject.SetActive(false);
        ruleTxt.gameObject.SetActive(true);
        backToMenu.gameObject.SetActive(true);

        joinSection.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
        btn_Host.gameObject.SetActive(false);
        btn_Client.gameObject.SetActive(false);
        partyJoinCode.gameObject.SetActive(false);
        joinCodeInput.gameObject.SetActive(false);
    }

    private void BackToMenu()
    {
        ruleButton.gameObject.SetActive(true);
        ruleTxt.gameObject.SetActive(false);
        backToMenu.gameObject.SetActive(false);
        
        btn_Host.gameObject.SetActive(true);
        joinSection.gameObject.SetActive(true);
        btn_Client.gameObject.SetActive(false);
        partyJoinCode.gameObject.SetActive(false);
        joinCodeInput.gameObject.SetActive(false);
    }

    private bool hasClickedEscape = false;
    private void Update()
    {
        if (GameManager.Instance.PlayerControllers.Count < 0)  partyJoinCode.gameObject.SetActive(false);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            hasClickedEscape = !hasClickedEscape;
            quitButton.gameObject.SetActive(hasClickedEscape);
        }
    }
}