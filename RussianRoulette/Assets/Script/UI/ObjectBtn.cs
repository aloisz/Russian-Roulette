using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ObjectBtn : MonoBehaviour
{
    
    private Button btn;
    private CanvasGroup canvasGroup;
    public TextMeshProUGUI text;
    public int OwnedByClientID;
    
    void Awake()
    {
        btn = transform.GetComponent<Button>();
        canvasGroup = transform.GetComponent<CanvasGroup>();
        text = transform.GetComponentInChildren<TextMeshProUGUI>();
        
        btn.onClick.AddListener((() => GameManager.Instance.PlayerControllers[OwnedByClientID].PlayerHUD.PressBtn(btn)));
    }

    private void OnDestroy()
    {
        GameManager.Instance.PlayerControllers[OwnedByClientID].PlayerHUD.ObjectBtns.Remove(this);
        GameManager.Instance.PlayerControllers[OwnedByClientID].PlayerHUD.Buttons.Remove(btn);
        GameManager.Instance.PlayerControllers[OwnedByClientID].PlayerHUD.btnCanvasGroup.Remove(canvasGroup);
    }

    public void EnableButton(float time)
    {
        btn.enabled = true;
        canvasGroup.DOFade(1, time);
    }

    public void DisableButton(float time)
    {
        btn.enabled = false;
        canvasGroup.DOFade(0, time).OnComplete((() => Destroy(gameObject)));
    }
    
}
