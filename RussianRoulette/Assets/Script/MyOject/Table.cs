using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Table : NetworkBehaviour
{
    public List<ObjectOnTable> ObjectsOnTable;
    
    
    public List<Tiles> tilesClient0;
    public NetworkVariable<int> tilesClient0Index = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    public List<Tiles> tilesClient1;
    public NetworkVariable<int> tilesClient1Index = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [Space] public List<ObjAvailable> allObjs;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        tilesClient0Index.OnValueChanged += (value, newValue) => tilesClient0Index.Value = newValue;
        tilesClient1Index.OnValueChanged += (value, newValue) => tilesClient1Index.Value = newValue;
    }

    public int SelectAnObject()
    {
        int result = 0;
        
        int randomValue = Random.Range(0, allObjs.Count);
        result = (int)allObjs[randomValue].ObjEnum;
        
        return result;
    }
    
    public void SpawnObjOnTable(int numberOfObjToSpawn, int cliendID)
    {
        for (int i = 0; i < numberOfObjToSpawn; i++)
        {
            ObjectOnTable obj = Instantiate(GameManager.Instance.objectOnTables[0], Vector3.zero, Quaternion.identity);
            var objNetworkObject = obj.GetComponent<NetworkObject>();
            objNetworkObject.Spawn();

            if (cliendID == 0)
            {
                obj.transform.position = tilesClient0[tilesClient0Index.Value].transform.position;
                obj.SetBasePos_Rpc(tilesClient0[tilesClient0Index.Value].transform.position);
                obj.SetClientInfo_Rpc(cliendID, tilesClient0Index.Value);
                tilesClient0[tilesClient0Index.Value].obj = obj;
                tilesClient0Index.Value++;
            }
            else
            {
                obj.transform.position = tilesClient1[tilesClient1Index.Value].transform.position;
                obj.SetBasePos_Rpc(tilesClient1[tilesClient1Index.Value].transform.position);
                obj.SetClientInfo_Rpc(cliendID, tilesClient1Index.Value);
                tilesClient1[tilesClient1Index.Value].obj = obj;
                tilesClient1Index.Value++;
            }
        }
    }
}

[System.Serializable]
public class Tiles
{
    public Transform transform;
    public ObjectOnTable obj;
}

[System.Serializable]
public class ObjAvailable
{
    public ObjEnum ObjEnum;
    public float weight;
}

[System.Serializable]
public enum ObjEnum
{
    Saw,
    Cigarette,
    Liquor,
    Wen,
    Phone,
    Adrenaline,
    Scissors
}
