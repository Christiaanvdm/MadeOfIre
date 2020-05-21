using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
namespace Complete
{
    public class RoomManager : MonoBehaviour
    {

        public List<Transform> spawnPoints;
        public List<Transform> exits;
        private Transform entrance;
        public List<GameObject> roomEnemies = new List<GameObject>();
        public List<DoorManager> doors = new List<DoorManager>();
        public int enemyCount;
        private PlayerManager playerManager;
        private GameObject rewardPodium;
        private int roomState = 0; //0 - New ; 1 - Enemies spawned ; 2 - Room cleared
        public int mazeDepth;
        private int extraSpawnPoints = 0;
        private int maxDepth = 30;
        private List<GameObject> enemiesPhase1;

        private List<GameObject> enemiesPhase2;

        private List<GameObject> enemiesPhase3;

        void Start()
        {
            entrance = transform.Find("Entrance");
            enemiesPhase1 = FindObjectsOfType<GameObject>().Where(x => x.tag == "enemy").ToList();
            //rewardPodium = transform.Find("Podium").gameObject;
            playerManager = FindObjectOfType<PlayerManager>();
            //rewardPodium.SetActive(false);
            enemiesPhase1.ForEach(x => x.SetActive(false));
        }

        void SetupPhases() {

        }

        public void InitializeMap(int _mazeDepth)
        {
            mazeDepth = _mazeDepth;
            mazeDepth += 1;
            if (mazeDepth > maxDepth)
                return;
            SpawnHallways(mazeDepth);
        }



        public void SpawnRoom(int mazeDepth)
        {
            var rooms = Resources.LoadAll<GameObject>("Rooms/South");
            var room = rooms.First(x => x.name == "ARoom2");
            GameObject newRoom = Instantiate(room) as GameObject;
            var entrance = transform.parent.Find("ExitN");

            newRoom.transform.position = entrance.position + (newRoom.transform.position - newRoom.transform.Find("Entrance").position);
            newRoom.transform.Find("RoomArea").GetComponent<RoomManager>().InitializeMap(mazeDepth);
        }

        public void SpawnHallways(int mazeDepth)
        {
            foreach (Transform exit in exits)
            {
                SpawnHallway(exit);
            }

        }

        public void SpawnHallway(Transform exit)
        {
            var rooms = Resources.LoadAll<GameObject>("Hallways/North");
            var room = rooms.First(x => x.name == "Straight");
            GameObject newRoom = Instantiate(room) as GameObject;

            newRoom.transform.position = exit.position + (newRoom.transform.position - newRoom.transform.Find("Entrance").position);
            //newRoom.GetComponent<ConnectorInit>().SpawnRoom(mazeDepth);
        }


        void CreateExtraSpawnPoints()
        {
            Transform transformMin, transformMax;
            transformMin = transform.parent.Find("Minimum");
            transformMax = transform.parent.Find("Maximum");

            for (int i = 0; i < extraSpawnPoints; i++)
            {

                var countOut = 0;
                var pointIsValid = false;
                while (!pointIsValid && countOut < 1000)
                {
                    countOut++;
                    var randomPoint = new Vector3(Random.Range(transformMin.position.x, transformMax.position.x), 0.4f, Random.Range(transformMin.position.z, transformMax.position.z));
                    var overlaps = Physics.OverlapSphere(randomPoint, 1f);
                    if (overlaps.Length == 2)
                    {
                        var newSpawn = new GameObject();

                        newSpawn.transform.position = randomPoint;
                        spawnPoints.Add(newSpawn.transform);

                        pointIsValid = true;
                    };
                }


            }
        }


        // Update is called once per frame
        void Update()
        {

        }

        private void FixedUpdate()
        {
        }

        void spawnEnemies()
        {
            enemiesPhase1.ForEach(x => x.SetActive(true));
        }

        void closeDoors()
        {
            foreach (DoorManager door in doors)
            {
                door.CloseDoor();
            }

            playerManager.DiscardHand();
            playerManager.InCombat = true;
        }

        void OpenDoors()
        {
            foreach (DoorManager door in doors)
            {
                door.OpenDoor();
            }
            //rewardPodium.SetActive(true);
            //LightComplete.SetActive(true);
            playerManager.InCombat = false;
        }


        public void StartFight()
        {
            if (roomState == 0)
            {
                roomState = 1;
                //CreateExtraSpawnPoints();
                spawnEnemies();
                closeDoors();
                gameObject.layer = 16;
            }
        }
    }
}