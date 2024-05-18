using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.Netcode;

public class PlayerHUD : MonoBehaviour
{
    public GameObject btnGO;
    [SerializeField] private Transform poolOfBtn;
    public int ownedByClientID; 
    
    [Space]
    public List<ObjectBtn> ObjectBtns;
    public List<Button> Buttons;
    public List<CanvasGroup> btnCanvasGroup;

    public MyObject selectedObject;
    
    
    public MyObject GetTheSelectedObj(MyObject myObject)
    {
        return selectedObject = myObject;
    }

    public void SetPlayerId(int Id)
    {
        if(Id == 0) transform.rotation *= Quaternion.Euler(new Vector3(0,0,0));
        else transform.rotation *= Quaternion.Euler(new Vector3(0,180,0));
    }

    private int GetPlayerID()
    {
        return ownedByClientID;
    }

    private int BtnsRotation()
    {
        var result = 0;
        result = GetPlayerID() == 0 ? 0 : 180;
        return result;
    }
    
    public void DisplayBtns(bool verif, List<HUD_OBJ> hudObj)
    {
        if (verif)
        {
            for (int i = 0; i < hudObj.Count; i++)
            {
                GameObject btn = Instantiate(btnGO, hudObj[i].HUDObjSpecs[GetPlayerID()].Transforms.position, hudObj[i].HUDObjSpecs[GetPlayerID()].Transforms.rotation * Quaternion.Euler(Vector3.up), poolOfBtn);
                btn.name = hudObj[i].actionName;
                btn.transform.rotation *= Quaternion.Euler(new Vector3(90,BtnsRotation(),0));
                
                ObjectBtns.Add(btn.GetComponent<ObjectBtn>()); 
                Buttons.Add(btn.GetComponent<Button>());
                btnCanvasGroup.Add(btn.GetComponent<CanvasGroup>());
                
                ObjectBtns[i].EnableButton(1);
                ObjectBtns[i].text.text = hudObj[i].actionName;
                ObjectBtns[i].OwnedByClientID = ownedByClientID;

                ObjectBtns[i].TargetClientID = hudObj[i].HUDObjSpecs[GetPlayerID()].targetClientID;
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

    public void PressBtn(Button btn, int targetClientID, int damage)
    {
        DisplayBtns(false, null);
        selectedObject.ChangeIsSelectedValue_Rpc();
        Effect(targetClientID,  damage);
    }

    private void Effect(int targetClientID, int damage)
    {
        switch (selectedObject.objAction)
        {
            case ObjAction.Normal:
                break;
            case ObjAction.EndingRound:
                GameManager.Instance.RoundEnded(targetClientID, damage);
                break;
        }
    }
}
