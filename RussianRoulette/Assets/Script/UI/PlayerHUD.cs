using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.Netcode;

public class PlayerHUD : NetworkBehaviour
{
    public GameObject btnGO;
    [SerializeField] private Transform poolOfBtn;
    public int ownedByClientID; 
    
    [Space]
    public List<ObjectBtn> ObjectBtns;
    public List<Button> Buttons;
    public List<CanvasGroup> btnCanvasGroup;

    public MyObject selectedObject;

    public static PlayerHUD Instance;
    
    public override void OnNetworkSpawn()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public MyObject GetTheSelectedObj(MyObject myObject)
    {
        return selectedObject = myObject;
    }

    public int SetPlayerId(int Id)
    {
        if(Id == 0) transform.rotation *= Quaternion.Euler(new Vector3(90,0,0));
        else transform.rotation *= Quaternion.Euler(new Vector3(90,180,0));
        return ownedByClientID = Id;
    }

    public int GetPlayerID()
    {
        return ownedByClientID;
    }

    public void EnableHUD(bool verif)
    {
        if(!IsOwner) return;
        gameObject.SetActive(verif);
    }

    public void DisplayBtns(bool verif, List<HUD_OBJ> hudObj)
    {
        if(!IsOwner) return;
        if (verif)
        {
            for (int i = 0; i < hudObj.Count; i++)
            {
                GameObject btn = Instantiate(btnGO, hudObj[i].transform.position, hudObj[i].transform.rotation * Quaternion.Euler(Vector3.up), poolOfBtn);
                btn.name = hudObj[i].actionName;
                btn.transform.rotation *= Quaternion.Euler(new Vector3(90,0,0));
                
                ObjectBtns.Add(btn.GetComponent<ObjectBtn>()); 
                Buttons.Add(btn.GetComponent<Button>());
                btnCanvasGroup.Add(btn.GetComponent<CanvasGroup>());
                
                ObjectBtns[i].EnableButton(0);
                ObjectBtns[i].text.text = hudObj[i].actionName;
                ObjectBtns[i].OwnedByClientID = ownedByClientID;

            }
        }
        else
        {
            foreach (var ObjectBtns in ObjectBtns)
            {
                ObjectBtns.DisableButton(0);
            }
        }
    }

    public void PressBtn(Button btn)
    {
        if(!IsOwner) return;
        DisplayBtns(false, null);
        selectedObject.ChangeIsSelectedValue_Rpc();
        Effect();
    }

    private void Effect()
    {
        switch (selectedObject.objAction)
        {
            case ObjAction.Normal:
                break;
            case ObjAction.EndingRound:
                GameManager.Instance.RoundEnded_Rpc();
                break;
        }
    }
}
