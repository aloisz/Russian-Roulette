using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[System.Serializable]
public struct Bullet : INetworkSerializeByMemcpy
{
    public BulletType bulletType;
    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref bulletType);
    }
}

[System.Serializable]
public enum BulletType
{
    Blank,
    Live
}
