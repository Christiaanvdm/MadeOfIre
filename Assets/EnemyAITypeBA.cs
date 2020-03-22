using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Complete
{
    //Boss Type A


    public class EnemyAITypeBA : MonoBehaviour
    {
        private int behavior = 0;
        private float speed = 2f;
        private float angular_speed = 120f;
        private float min_player_distance;
        private GameObject player;
        public GameObject enemySprite;

        private NavMeshAgent navMeshAgent;
        // Start is called before the first frame update
        void Start()
        {
            LoadSprite();
            navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
            player = GameObject.Find("Player");
            StartCoroutine("BeginFight");
           
        }

        protected virtual void LoadSprite()
        {
        
            GameObject spriteExample = Resources.Load("EnemySpriteBA") as GameObject;
            enemySprite = Instantiate(spriteExample, gameObject.transform.position, spriteExample.transform.rotation);
            enemySprite.GetComponent<EnemySpriteManager>().enemyBA = this;

        }

        private void SetupNavMeshAgent()
        {
            navMeshAgent.speed = speed;
            navMeshAgent.angularSpeed = angular_speed;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void SnapSpriteToEnemy()
        {
            enemySprite.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        }

        private void FixedUpdate()
        {
            SnapSpriteToEnemy();
            if (behavior == 1)
            {
                KeepDistance();
            }
        }

        IEnumerator BeginFight()
        {
            yield return StartCoroutine("StartFight");
            yield return StartCoroutine("EvadeAndShoot");


        }
        // 
        IEnumerator StartFight()
        {
          
            yield return null;
        }

        IEnumerator EvadeAndShoot()
        {
            behavior = 1;
            yield return new WaitForSeconds(15f);
            yield return null;
        }

        IEnumerator Wednesday()
        {
            yield return null;
        }

        IEnumerator Thursday()
        {
            yield return null;
        }

        IEnumerator Friday()
        {
            yield return null;
        }

        IEnumerator Saturday()
        {
            yield return null;
        }

        IEnumerator Sunday()
        {
            yield return null;
        }

        private void KeepDistance()
        {
            Vector3 toPlayer = player.transform.position - transform.position;
            if (Vector3.Distance(player.transform.position, transform.position) < 1)
            {
                Vector3 targetPosition = toPlayer.normalized * -1f;
                navMeshAgent.SetDestination(targetPosition);
            }
        }

    }


    enum Phase
    {
        StartFight = 0,
        Tuesday = 1,
        Wednesday = 2,
        Thursday = 3,
        Friday = 4,
        Saturday = 5,
        Sunday = 6
    }

    enum Behaviour
    {
        KeepDistance = 1,
    }


    public static class EnumUtil
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}