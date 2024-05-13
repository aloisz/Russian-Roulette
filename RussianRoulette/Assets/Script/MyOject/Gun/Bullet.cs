using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    public BulletType bulletType;
}

public enum BulletType
{
    Blank,
    Live
}
