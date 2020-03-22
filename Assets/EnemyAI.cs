using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Complete {
    public class EnemyAI : MonoBehaviour
    {
        private float nextUpdate = 1f;
        private float nextUpdate2 = 1f;
        private Vector3 startingRotation;
        private GameObject player;
        private Animator anim;
        private EnemyManager enemyManager;
        public float projectileSpeed = 10f;
        public float moveSpeed = 10f;
        private float maximumTrackingSpeed = 3f;
        private bool finalDeath = false;
        private Rigidbody rigidBody;
        private Vector3 originalUp;
        private float individual_movespeed;
        private float geneticVarianceMin = 0.4f;
        private float geneticVarianceMax = 1f;
        private float trackingSpeed;
        // Start is called before the first frame update
        void Start()
        {
            CreateGenetics();
            originalUp = transform.up;
            rigidBody = gameObject.GetComponent<Rigidbody>();
            enemyManager = gameObject.GetComponent<EnemyManager>();
            //anim = gameObject.GetComponent<Animator>();
            player = GameObject.Find("Player");
        }

        void CreateGenetics()
        {
            trackingSpeed = maximumTrackingSpeed * Random.Range(geneticVarianceMin, geneticVarianceMax);
            individual_movespeed = moveSpeed * Random.Range(geneticVarianceMin, geneticVarianceMax);
        }
        public float duration = 0.5f;
        public float duration2 = 3f;
        // Update is called once per frame
        void Update()
        {

            if (Time.time >= nextUpdate)
            {
                nextUpdate = Time.time + duration;
                UpdateEveryDuration();
                duration = Random.Range(1f, 2f);
            }

            if (Time.time >= nextUpdate2)
            {
                nextUpdate2 = Time.time + duration2;
                UpdateEveryDuration2();
                duration = Random.Range(1f, 2f);
            }

        }

        void UpdateEveryDuration2()
        {
            if (!enemyManager.isAlive)
            {
                finalDeath = true;
            }
            else
            {
                Attack();
            }

        }
        private void FixedUpdate()
        {
            if (!(finalDeath))
            {
                LookAtPlayer();
                MoveForward();
            }
           
          

        }

        float angle;

        void LookAtPlayer()
        {

            Vector3 targetDir = player.transform.position - transform.position;
            Vector3 forward = transform.forward;
            Vector3 localTarget = transform.InverseTransformPoint(player.transform.position);

            angle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

            Vector3 eulerAngleVelocity = new Vector3(0, angle, 0);
            Quaternion deltaRotation = Quaternion.Euler(eulerAngleVelocity * Time.deltaTime * trackingSpeed);
            rigidBody.MoveRotation(rigidBody.rotation * deltaRotation);
            //rigidbody.MoveRotation(rigidbody.rotation * deltaRotation);
            ////anim.SetBool("Walk_Anim", true);
            //Vector3 playerPosition = player.transform.position;
            //Vector3 currentPosition = transform.position;
            //Vector3 playerDirection = (playerPosition - currentPosition).normalized;
            //rigidbody.rotation = Quaternion
            //transform.forward = Vector3.Lerp(transform.forward, playerDirection, Time.deltaTime);
        }

        void UpdateEveryDuration()
        {
            
            //Attack();
        }

        void Attack()
        {
            enemyManager.fireShell(projectileSpeed);
        }


        void MoveForward()
        {

            //print("Move forward");
            //transform.up = originalUp;
            rigidBody.velocity = transform.forward * individual_movespeed;

        }
    }
}