using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectBtn : MonoBehaviour
{

    private Button btn;
    private CanvasGroup canvasGroup;

    private TextMeshProUGUI text;
    void Start()
    {
        btn = transform.GetComponent<Button>();
        canvasGroup = transform.GetComponent<CanvasGroup>();
        text = transform.GetComponentInChildren<TextMeshProUGUI>();
        
        btn.onClick.AddListener((() => HUD.Instance.PressBtn(btn)));
        
        HUD.Instance.Buttons.Add(btn);
        HUD.Instance.btnCanvasGroup.Add(canvasGroup);
        
    }

    
}
