using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectBtn : MonoBehaviour
{

    private Button btn;
    private CanvasGroup canvasGroup;
    public TextMeshProUGUI text;
    
    void Awake()
    {
        btn = transform.GetComponent<Button>();
        canvasGroup = transform.GetComponent<CanvasGroup>();
        text = transform.GetComponentInChildren<TextMeshProUGUI>();
        
        btn.onClick.AddListener((() => HUD.Instance.PressBtn(btn)));
        
        //HUD.Instance.ObjectBtns.Add(this);
        //HUD.Instance.Buttons.Add(btn);
        //HUD.Instance.btnCanvasGroup.Add(canvasGroup);
    }

    private void OnDestroy()
    {
        HUD.Instance.ObjectBtns.Remove(this);
        HUD.Instance.Buttons.Remove(btn);
        HUD.Instance.btnCanvasGroup.Remove(canvasGroup);
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
