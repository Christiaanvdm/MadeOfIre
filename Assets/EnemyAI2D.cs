//using System.Collections;
//using System.Collections.Generic;

//using UnityEngine;
//using UnityEngine.AI;

//namespace Complete
//{
//    public class EnemyAI2D : MonoBehaviour
//    {
//        private float nextUpdate = 1f;
//        private float nextUpdate2 = 1f;
//        private Vector3 startingRotation;
//        private GameObject player;
//        private Animator anim;
//        protected EnemyManager2D enemyManager;
//        public float projectileSpeed = 10f;
//        private float moveSpeed = 2f;
//        private float maximumTrackingSpeed = 120f;
//        private bool finalDeath = false;
//        private Rigidbody rigidBody;
//        private Vector3 originalUp;
//        public float individual_movespeed;
//        private float starting_movespeed;
//        private float geneticVarianceMin = 0.8f;
//        private float geneticVarianceMax = 2f;
//        private float trackingSpeed;
//        private GameObject body;
//        public GameObject enemySprite;
//        private NavMeshAgent navMeshAgent;
//        private Rigidbody projectileRigidbody;
//        private Transform colliderTransform;
//        // Start is called before the first frame update
//        void Start()
//        {
//            colliderTransform = transform.Find("Collider");

//            LoadSprite();
//            SetupBase();
//            originalUp = transform.up;
//            rigidBody = gameObject.GetComponent<Rigidbody>();

//            player = GameObject.Find("Player");
//            anim = enemySprite.transform.Find("Sprite").gameObject.GetComponent<Animator>();
//            anim.SetBool("Moving", true);
//            navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
//            projectileRigidbody = GameObject.Find("EnemyProjectile").GetComponent<Rigidbody>();
//            CreateGenetics();
//            //body = transform.Find("Body").gameObject;
//        }

//        protected virtual void LoadSprite()
//        {
//            enemyManager = gameObject.GetComponent<EnemyManager2D>();
//            GameObject spriteExample = Resources.Load("EnemySprite") as GameObject;
//            enemySprite = Instantiate(spriteExample, gameObject.transform.position, spriteExample.transform.rotation);
//            enemySprite.GetComponent<EnemySpriteManager>().enemy = enemyManager;

//        }

//        protected virtual void SetupBase()
//        {

//        }

//        void CreateGenetics()
//        {
//            trackingSpeed = maximumTrackingSpeed * Random.Range(geneticVarianceMin, geneticVarianceMax);
//            individual_movespeed = moveSpeed * Random.Range(geneticVarianceMin, geneticVarianceMax);
//            starting_movespeed = individual_movespeed;
//            navMeshAgent.speed = individual_movespeed;
//            navMeshAgent.angularSpeed = trackingSpeed;
//            nextUpdate = nextUpdate * Random.Range(geneticVarianceMin, geneticVarianceMax);

//            nextUpdate2 = nextUpdate2 * Random.Range(geneticVarianceMin, geneticVarianceMax);
//        }
//        public float duration = 0.5f;
//        public float duration2 = 3f;
//        // Update is called once per frame
//        void Update()
//        {

//            if (Time.time >= nextUpdate)
//            {
//                nextUpdate = Time.time + duration;
//                UpdateEveryDuration();
//                duration = duration * Random.Range(geneticVarianceMin, geneticVarianceMax);
//            }

//            if (Time.time >= nextUpdate2)
//            {
//                nextUpdate2 = Time.time + duration2;
//                UpdateEveryDuration2();
//                duration2 = duration2 * Random.Range(geneticVarianceMin, geneticVarianceMax);
//            }

//        }
//        public void Death()
//        {
//            finalDeath = true;
//            anim.SetInteger("State", 5);
//            enemySprite.layer = 16;
//            StartCoroutine("SlightlyAfterDeath");
//        }

//        IEnumerator SlightlyAfterDeath()
//        {
//            yield return new WaitForSeconds(0.5f);
//            gameObject.SetActive(false);
//            yield return null;
//        }
//        void UpdateEveryDuration2()
//        {

//            if (!enemyManager.isAlive)
//            {
//                finalDeath = true;
//            }
//            else
//            {
//                if (EnemyLineOfSight())
//                {
//                    FireAtEnemy();

//                }
//            }

//        }
//        private void FixedUpdate()
//        {
//            if (!(finalDeath))
//            {
//                LookAtPlayer();
//                MoveTowardsEnemy();
//            }
//            SnapSpriteToEnemy();
//            transform.position = new Vector3(transform.position.x, 1.4f, transform.position.z);
//            //LookAtPlayer();


//        }
//        virtual public void FireAtEnemy()
//        {
//            Vector3 shotDirection = (transform.position - player.transform.position).normalized;

//            Rigidbody projectileInstance = Instantiate(projectileRigidbody, transform.position, transform.rotation) as Rigidbody;
//            projectileInstance.velocity = -1 * shotDirection * projectileSpeed;

//            projectileInstance.transform.up = new Vector3(0, 1, 0);
//            AttackProjectile nextAP = projectileInstance.GetComponent<AttackProjectile>();
//            nextAP.speed = projectileSpeed;
//            //combatManager.ModifyProjectile(ref nextAP);
//            //nextAP.StartUp();
//        }
//        private void SnapSpriteToEnemy()
//        {
//            enemySprite.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

//        }



//        float angle;

//        private void MoveTowardsEnemy()
//        {
//            navMeshAgent.SetDestination(player.transform.position);
//        }

//        void LookAtPlayer()
//        {

//            transform.LookAt(player.transform);
//            updateOrientation((player.transform.position - transform.position).normalized);

//        }

//        private void updateOrientation(Vector3 direction)
//        {
//            // Look at mouse
//            if ((direction.x > 0) && (direction.z > 0))
//            {
//                if (direction.x < direction.z)
//                {
//                    lookUp();
//                }
//                else
//                {
//                    lookRight();
//                }
//            }
//            if ((direction.x < 0) && (direction.z > 0))
//            {
//                if (direction.x > direction.z * -1)
//                {
//                    lookUp();
//                }
//                else
//                {
//                    lookLeft();
//                }
//            }
//            if ((direction.x < 0) && (direction.z < 0))
//            {
//                if (direction.x > direction.z)
//                {
//                    lookDown();
//                    //print("Down");
//                }
//                else
//                {
//                    lookLeft();
//                    //print("Left");
//                }
//            }
//            if ((direction.x > 0) && (direction.z < 0))
//            {
//                if (direction.x < direction.z * -1)
//                {
//                    lookDown();
//                    //print("Down");
//                }
//                else
//                {
//                    lookRight();
//                    //print("Right");
//                }
//            }
//        }
//        private string current_orientation;
//        private void lookUp()
//        {

//            if (current_orientation != "up")
//            {
//                anim.SetInteger("State", 1);

//                current_orientation = "up";
//            }
//        }

//        private void lookDown()
//        {
//            if (current_orientation != "down")
//            {
//                anim.SetInteger("State", 3);
//                current_orientation = "down";
//            }
//        }

//        private void lookRight()
//        {
//            if (current_orientation != "right")
//            {
//                anim.SetInteger("State", 2);
//                current_orientation = "right";
//            }
//        }

//        private void lookLeft()
//        {
//            if (current_orientation != "left")
//            {
//                anim.SetInteger("State", 4);
//                current_orientation = "left";
//            }
//        }

//        void UpdateEveryDuration()
//        {

//            //Attack();
//        }

//        void Attack()
//        {
//            enemyManager.fireShell(projectileSpeed);
//        }

//        private bool EnemyLineOfSight()
//        {
//            RaycastHit[] raycastHits;
//            Vector3 rayDirection = player.transform.position - transform.position ;
//            raycastHits = Physics.RaycastAll(transform.position, rayDirection, Vector3.Distance(transform.position, player.transform.position));
//            bool canSeeEnemy = true;
//            foreach (RaycastHit nextHit in raycastHits)
//            {
//                if (nextHit.collider.gameObject.layer == 12)  // 12 = World
//                {
//                    canSeeEnemy = false;
//                }
//            }
//            return canSeeEnemy;
//        }


//    }
//}