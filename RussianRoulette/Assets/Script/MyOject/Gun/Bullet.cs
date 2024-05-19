using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class Bullet : NetworkBehaviour
{
    public NetworkVariable<int> bulletID = new NetworkVariable<int>
        (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<BulletType> bulletType = new NetworkVariable<BulletType>
        (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [SerializeField] private List<GameObject> bucks;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        bulletID.OnValueChanged += (value, newValue) => bulletID.Value = newValue;
        bulletType.OnValueChanged += OnIsBulletType_Rpc;
        
        
        GameManager.Instance.presentedBullets.Add(this);
        
    }

    [Rpc(SendTo.Everyone)]
    private void OnIsBulletType_Rpc(BulletType previousvalue, BulletType newvalue)
    {
        Debug.Log(bulletType.Value);
        switch (bulletType.Value)
        {
            case BulletType.Live:
                bucks[1].SetActive(false);
                break;
            case BulletType.Blank:
                bucks[0].SetActive(false);
                break;
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        bulletID.OnValueChanged -= (value, newValue) => bulletID.Value = newValue;
        bulletType.OnValueChanged -= (value, newValue) => bulletType.Value = newValue;
        
        GameManager.Instance.presentedBullets.Remove(this);
    }
}

public enum BulletType
{
    NUll,
    Blank,
    Live
}
