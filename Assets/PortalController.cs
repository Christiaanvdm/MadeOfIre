using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Complete {
    public class PortalController : MonoBehaviour
    {
        private PlayerManager playerManager;
        // Start is called before the first frame update
        void Start()
        {
            playerManager = Transform.FindObjectOfType<PlayerManager>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "player")
            {

                SpawnRoom(0, other.transform);

            }
        }

        public void SpawnRoom(int mazeDepth, Transform position)
        {
            StartCoroutine(manageOverlayForTeleport(position));
        }

        IEnumerator manageOverlayForTeleport(Transform other) {
            var startTeleport = true;
            while (startTeleport)
            {
                playerManager.ControlsDisabled = true;
                var overlay = Transform.FindObjectOfType<OverlayController>();
                overlay.showOverlay();
                yield return new WaitForSeconds(0.25f);
                SceneManager.LoadScene("SampleScene");
                other.transform.position = other.transform.position + new Vector3(0, 0, 1000);
                var rooms = Resources.LoadAll<GameObject>("Rooms/South");
                var room = rooms.First(x => x.name == "ARoom2");
                GameObject newRoom = Instantiate(room) as GameObject;
                newRoom.transform.position = other.position + (newRoom.transform.position - newRoom.transform.Find("Entrance").position);
                newRoom.transform.position = new Vector3(newRoom.transform.position.x, -0.9f, newRoom.transform.position.z);
                yield return new WaitForSeconds(1f);
                overlay.hideOverlay();
                yield return new WaitForSeconds(0.5f);
                startTeleport = false;
                playerManager.ControlsDisabled = false;
                playerManager.SavePlayerCards();
            }
            yield return null;
        }

        private void saveRooms() {
            Room newRoom = new Room();
            newRoom.prefabName = "ARoom2";

        }
    }

    public class Room {
        public string prefabName;
        public int id;

    }

    public class Floor {
        List<Floor> floors = new List<Floor>();
        string name;
    }

    public class Level {
        List<Room> rooms = new List<Room>();
    }
}