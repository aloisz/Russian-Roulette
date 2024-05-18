using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class Bullet : NetworkBehaviour
{
    public NetworkVariable<int> bulletID= new NetworkVariable<int>
        (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    public NetworkVariable<BulletType> bulletType = new NetworkVariable<BulletType>
        (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        bulletID.OnValueChanged += (value, newValue) => bulletID.Value = newValue;
        bulletType.OnValueChanged += (value, newValue) => bulletType.Value = newValue;
        
        GameManager.Instance.presentedBullets.Add(this);
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
    Blank,
    Live
}
