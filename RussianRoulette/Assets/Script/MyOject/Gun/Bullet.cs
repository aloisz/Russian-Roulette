using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class Bullet : NetworkBehaviour
{
    public NetworkVariable<BulletType> bulletType = new NetworkVariable<BulletType>
        (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        GameManager.Instance.presentedBullets.Add(this);
    }
}

public enum BulletType
{
    Blank,
    Live
}
