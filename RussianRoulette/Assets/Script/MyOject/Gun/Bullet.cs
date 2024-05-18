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
        
        foreach (var buck in bucks)
        {
            buck.SetActive(false);
        }

        bulletID.OnValueChanged += (value, newValue) => bulletID.Value = newValue;
        bulletType.OnValueChanged += OnIsBulletType;
        
        
        GameManager.Instance.presentedBullets.Add(this);
    }

    private void OnIsBulletType(BulletType previousvalue, BulletType newvalue)
    {
        Debug.Log(bulletType.Value);
        switch (bulletType.Value)
        {
            case BulletType.Live:
                bucks[0].SetActive(true);
                break;
            case BulletType.Blank:
                bucks[1].SetActive(true);
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
