using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConnectorInit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnRoom() {
        var rooms = Resources.LoadAll<GameObject>("Rooms/South");
        var room = rooms.First();
        GameObject newRoom = Instantiate(room) as GameObject;
        var entrance = transform.Find("Entrance");

        newRoom.transform.position = entrance.position + ( newRoom.transform.position - newRoom.transform.Find("Entrance").position);
    }
}
