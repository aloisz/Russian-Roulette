using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.Netcode;

public class HUD : MonoBehaviour
{
    public GameObject btnGO;
    [SerializeField] private Transform poolOfBtn;
    
    [Space]
    public List<ObjectBtn> ObjectBtns;
    public List<Button> Buttons;
    public List<CanvasGroup> btnCanvasGroup;

    public MyObject selectedObject;
    public static HUD Instance;
    private void Awake()
    {
        Instance = this;
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
                GameObject btn = Instantiate(btnGO, hudObj[i].transform.position, hudObj[i].transform.rotation * Quaternion.Euler(Vector3.up), poolOfBtn);
                btn.name = hudObj[i].actionName;
                ObjectBtns.Add(btn.GetComponent<ObjectBtn>()); 
                Buttons.Add(btn.GetComponent<Button>());
                btnCanvasGroup.Add(btn.GetComponent<CanvasGroup>());
                
                ObjectBtns[i].EnableButton(1);
                ObjectBtns[i].text.text = hudObj[i].actionName;
                
            }
        }
        else
        {
            foreach (var ObjectBtns in ObjectBtns)
            {
                ObjectBtns.DisableButton(1);
            }
        }
    }

    public void PressBtn(Button btn)
    {
        DisplayBtns(false, null);
        selectedObject.ChangeIsSelectedValue();
        Effect();
    }

    private void Effect()
    {
        switch (selectedObject.ObjEffect)
        {
            case ObjEffect.Normal:
                break;
            case ObjEffect.EndingRound:
                GameManager.Instance.RoundEndedRpc();
                break;
        }
    }
}
