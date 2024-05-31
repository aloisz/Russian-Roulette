using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Player;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class Gun : MyObject
{
    [SerializeField] private Transform desiredPos;

    [SerializeField]
    private NetworkVariable<int> damageApplied = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server); 
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        damageApplied.OnValueChanged += (value, newValue) => damageApplied.Value = newValue;
    }
    
    
    protected override void Select(ulong OwnerClientId)
    { 
        if(GameManager.Instance.bulletNumber.Value == 0) return;
        base.Select(OwnerClientId);
        transform.DOMove(GameManager.Instance.PlayerControllers[(int)OwnerClientId].CameraManager.objPosition
            .position, .3f);
        desiredPos = GameManager.Instance.PlayerControllers[(int)OwnerClientId].CameraManager.objPosition;

        transform.DORotate((int)OwnerClientId == 0 ? Vector3.forward : new Vector3(0, 180, 0), .3f);

        GameManager.Instance.PlayerControllers[(int)OwnerClientId].CameraManager.ChangeState(StateCamera.PlayerPos);
        GameManager.Instance.PlayerControllers[(int)OwnerClientId].PlayerHUD.SetGunDamage(damageApplied.Value);

        SetBulletsNotVisible_Rpc();
    }

    [Rpc(SendTo.Everyone)]
    private void SetBulletsNotVisible_Rpc()
    {
        foreach (var bullet in GameManager.Instance.presentedBullets)
        {
            bullet.transform.GetChild(0).gameObject.SetActive(false);
            bullet.transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    [Rpc(SendTo.Server)]
    public void AddDamage_Rpc(int value)
    {
        damageApplied.Value += value;
    }

    public void ResetDamage()
    {
        damageApplied.Value = 1;
    }
    
    protected override void DeSelect(ulong OwnerClientId)
    {
        base.DeSelect(OwnerClientId);
        transform.DOMove(basePosition, .3f);
        transform.DORotate(baseRotation.eulerAngles, .3f);
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Color.red;
        GUIStyle style = new GUIStyle();
        style.fontSize = 40;
        style.fontStyle = FontStyle.Bold;
        Handles.Label(transform.position + new Vector3(0,.1f,0), damageApplied.Value.ToString(),style);
    }
    #endif
}
