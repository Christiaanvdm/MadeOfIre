using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace Complete {
    public class RoomManager : MonoBehaviour
    {

        public List<Transform> spawnPoints;
        private GameObject enemySample;
        private GameObject enemySampleA;
        private GameObject enemySampleBA;
        public List<GameObject> roomEnemies = new List<GameObject>();
        public List<DoorManager> doors = new List<DoorManager>();
        public int enemyCount;
        private PlayerManager playerManager;
        private GameObject rewardPodium;
        private int roomState = 0; //0 - New ; 1 - Enemies spawned ; 2 - Room cleared
        private GameObject LightComplete;

        private int extraSpawnPoints = 5; 
       
         void Start()
        {
            LightComplete = GameObject.Find("LightComplete");
            LightComplete.SetActive(false);
            rewardPodium = transform.Find("Podium").gameObject;
            enemySample = Resources.Load("Enemy", typeof(GameObject)) as GameObject;
            enemySampleA = Resources.Load("EnemyA", typeof(GameObject)) as GameObject;
            enemySampleBA = Resources.Load("EnemyBA", typeof(GameObject)) as GameObject;
            playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
            rewardPodium.SetActive(false);
            
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
                    var randomPoint = new Vector3(Random.Range(transformMin.position.x, transformMax.position.x), 1.2f, Random.Range(transformMin.position.z, transformMax.position.z));
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
            if ((roomState != 0) && (enemyCount == 0)) {
                OpenDoors();
                rewardPodium.SetActive(true);
            }
        }

        void SpawnEnemy(Vector3 position, Quaternion rotation)
        {
            GameObject newEnemy = Instantiate(enemySample, position, rotation) as GameObject;

            newEnemy.transform.SetParent(transform);

            roomEnemies.Add(newEnemy);
        }

        void spawnEnemies()
        {
            enemyCount = spawnPoints.Count;
            foreach (Transform nextPoint in spawnPoints)
            {
                SpawnEnemy(nextPoint.position + new Vector3(0, 0f, 0), nextPoint.rotation);
            }
            foreach (GameObject enemy in roomEnemies)
            {
                enemy.transform.rotation = Quaternion.identity;
            }
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
            rewardPodium.SetActive(true);
            LightComplete.SetActive(true);
            playerManager.InCombat = false;
        }
        private void OnTriggerEnter(Collider other)
        {
            if ((other.gameObject.name == "Player") && roomState == 0)
            {
                roomState = 1;
                CreateExtraSpawnPoints();
                spawnEnemies();
                closeDoors();
                gameObject.layer = 16;
            }
        }
    }
}