using Complete;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightStartTrigger : MonoBehaviour
{
    private RoomManager room;
    // Start is called before the first frame update
    void Start()
    {
        room = transform.GetComponentInParent<RoomManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.name == "Player"))
        {
            room.StartFight();
            gameObject.SetActive(false);
        }
    }
}
