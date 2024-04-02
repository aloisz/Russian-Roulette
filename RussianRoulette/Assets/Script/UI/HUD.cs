using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HUD : MonoBehaviour
{
    public GameObject btnGO;
    
    public List<Button> Buttons;
    public List<CanvasGroup> btnCanvasGroup;

    public MyObject selectedObject;
    public static HUD Instance;
    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator Start()
    {
        foreach (var Button in Buttons)
        {
            Button.enabled = false;
            Button.onClick.AddListener((() => PressBtn(Button)));
        }

        yield return null;
        foreach (var btn in btnCanvasGroup)
        {
            btn.DOFade(0, 0);
        }
    }

    public MyObject GetTheSelectedObj(MyObject myObject)
    {
        return selectedObject = myObject;
    }
    
    public void DisplayBtns(bool verif, List<HUD_OBJ> hudObj)
    {
        if (verif)
        {
            for (int i = 0; i < hudObj.Count; i++)
            {
                Buttons[i].enabled = true;
                Buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = hudObj[i].actionName;
            }

            foreach (var btn in btnCanvasGroup)
            {
                btn.DOFade(1, 1);
            }
        }
        else
        {
            foreach (var Button in Buttons)
            {
                Button.enabled = false;
            }

            foreach (var btn in btnCanvasGroup)
            {
                btn.DOFade(0, 1);
            }
        }
    }

    public void PressBtn(Button btn)
    {
        Debug.Log($"btn name {btn} ", this);
        DisplayBtns(false, null);
        selectedObject.ChangeIsSelectedValue();
    }

    private void Effect()
    {
        
    }
}
