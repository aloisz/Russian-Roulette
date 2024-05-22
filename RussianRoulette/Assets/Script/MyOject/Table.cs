using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Table : MonoBehaviour
{
    public List<Tiles> tilesClient0;
    private int tilesClient0Index;
    
    public List<Tiles> tilesClient1;
    private int tilesClient1Index;


    public void SpawnObjOnTable(int numberOfObjToSpawn, int cliendID)
    {
        for (int i = 0; i < numberOfObjToSpawn; i++)
        {
            ObjectOnTable obj = Instantiate(GameManager.Instance.objectOnTables[0], Vector3.zero, Quaternion.identity);
            var objNetworkObject = obj.GetComponent<NetworkObject>();
            objNetworkObject.Spawn();

            if (cliendID == 0)
            {
                obj.transform.position = tilesClient0[tilesClient0Index].transform.position;
                obj.SetBasePos(tilesClient0[tilesClient0Index].transform.position);
                obj.SetClientInfo_Rpc(cliendID, tilesClient0Index);
                tilesClient0[tilesClient0Index].obj = obj;
                tilesClient0Index++;
            }
            else
            {
                obj.transform.position = tilesClient1[tilesClient1Index].transform.position;
                obj.SetBasePos(tilesClient1[tilesClient1Index].transform.position);
                obj.SetClientInfo_Rpc(cliendID, tilesClient1Index);
                tilesClient1[tilesClient1Index].obj = obj;
                tilesClient1Index++;
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
