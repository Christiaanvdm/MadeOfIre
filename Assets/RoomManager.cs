using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
                spawnEnemies();
                closeDoors();
                gameObject.layer = 16;
            }
        }
    }
}