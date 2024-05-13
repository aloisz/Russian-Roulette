using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public BulletType bulletType;
}

public enum BulletType
{
    Blank,
    Live
}
