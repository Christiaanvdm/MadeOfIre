using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Complete
{
    public class AttackProjectile : MonoBehaviour
    {
        public float damage = 10f;
        public float scale = 1f;
        public float speed = 0f;
        public float knockback = 0.1f;
        public float daze_duration = 0.5f;
        private bool alive = true;
        public Rigidbody rigidBody;
        private GameObject sprite;
        public AudioSource CollisionAudio;
        public GameObject player;
        private Quaternion originalRotation;
        public Projectile projectile;
        private CombatManager combatManager;

        private GameObject childSprite;
        private GameObject exampleProjectile;
        private GameObject exampleSprite;


        public List<AttackModifier> enemyModifiers = new List<AttackModifier>();
        public List<AttackModifier> birthModifiers = new List<AttackModifier>();
        public List<AttackModifier> allAttackModifiers = new List<AttackModifier>();
        public List<AttackModifier> hitModifiers = new List<AttackModifier>();
        private void resetSpriteOrientation()
        {
            childSprite.transform.SetPositionAndRotation(childSprite.transform.position, exampleSprite.transform.rotation);
        }

        // Start is called before the first frame update
        void Start()
        {
            combatManager = GameObject.Find("SceneManager").GetComponent<CombatManager>();
            originalRotation = gameObject.transform.rotation;
            player = GameObject.Find("Player");
            sprite = gameObject.transform.Find("Sprite").gameObject;
            rigidBody = gameObject.GetComponent<Rigidbody>();

        }

        public void updateScale(float multiplier)
        {
            scale = scale * multiplier;
        }
        // Update is called once per frame
        void Update()
        {

        }

        private void FixedUpdate()
        {
            if (!alive)
            {
                Despawn();
            }


        }

        public void AddModifier(AttackModifier newAM)
        {
            if (newAM.context == "Birth") {
                birthModifiers.Add(newAM); }
            else if (newAM.context == "Enemy")
            {
                enemyModifiers.Add(newAM);
            }
            else if (newAM.context == "Hit")
            {
                hitModifiers.Add(newAM);
            }

        }

        //void Attack(Vector3 shotDirection)
        //{
        //    // Generate projectile
        //    Projectile newProjectile = new Projectile();

        //    Rigidbody projectileInstance = Instantiate(rigidBody, transform.position, transform.rotation) as Rigidbody;
        //    newProjectile.rigidbody = projectileInstance;

        //    projectileInstance.velocity = shotDirection * speed;
        //    newProjectile.rigidbody.gameObject.GetComponent<AttackProjectile>().projectile = newProjectile;

        //    projectileInstance.transform.right = -1 * shotDirection;
        //    combatManager.ModifyProjectile(ref newProjectile);
        //    newProjectile.StartUp();

        //}

        public void StartUp()
        {
            exampleProjectile = GameObject.Find("AttackProjectile");
            childSprite = rigidBody.transform.Find("Sprite").gameObject;
            exampleSprite = exampleProjectile.transform.Find("Sprite").gameObject;
            //renderer = rigidbody.gameObject.GetComponent<Renderer>();
            //resetSpriteOrientation();
            Birth();

        }



        private void Birth()
        {
            Vector3 projectileForward = transform.right * -1;
            foreach (AttackModifier nextAM in birthModifiers)
            {
                if (nextAM.is_enabled && (nextAM.type == "split_shot"))
                {
                    Vector3 shotDirection = (FindMousePointRelativeToPlayer() - player.transform.position).normalized;
                    //print(shotDirection);
                    //Rigidbody projectileInstance = Instantiate(projectileRigidbody, originTransform.position, originTransform.rotation) as Rigidbody;
                    //projectileInstance.velocity = shotDirection * projectileSpeed;

                    //projectileInstance.transform.right = -1 * shotDirection;
                    //AttackProjectile nextAP = projectileInstance.GetComponent<AttackProjectile>();
                    //nextAP.speed = projectileSpeed;
                    //combatManager.ModifyProjectile(ref nextAP);
                    //nextAP.StartUp();
                    // Spawn two more projectiles
                    FireAnotherProjectile(offsetDirectionByAngle(rigidBody.velocity.normalized, 25), rigidBody.transform.position);
                    FireAnotherProjectile(offsetDirectionByAngle(rigidBody.velocity.normalized, -25), rigidBody.transform.position);
                }
            }
        }

        private Vector3 offsetDirectionByAngle(Vector3 directionForward, float angleDegrees)
        {
            return Quaternion.AngleAxis(angleDegrees, new Vector3(0, 1, 0)) * directionForward;
        }

        static void SetLayerOnAll(GameObject obj, int layer)
        {
            foreach (Transform trans in obj.GetComponentsInChildren<Transform>(true))
            {
                trans.gameObject.layer = layer;
                trans.gameObject.tag = "projectile_no_collide";
            }
        }

        private void FireAnotherProjectile(Vector3 shotDirection, Vector3 origin)
        {

            //SetLayerOnAll(exampleRigidBody.gameObject, 16);
            //AttackProjectile newAP = exampleRigidbody.gameObject.GetComponent<AttackProjectile>();
            Rigidbody projectileInstance = Instantiate(rigidBody, transform.position, transform.rotation) as Rigidbody;

            //SetLayerOnAll(newAP.gameObject, 16);
            AttackProjectile newAP = projectileInstance.gameObject.GetComponent<AttackProjectile>();
            projectileInstance.velocity = shotDirection.normalized * speed;
            newAP.hitModifiers.Clear();
            newAP.originalRotation = newAP.transform.rotation;

            projectileInstance.transform.up = new Vector3(0, 1, 0);
            //newAP.transform.forward = forwardDirection;

            newAP.birthModifiers.Clear();
            newAP.StartUp();
        }


        private void FireAnotherProjectileChain(Vector3 shotDirection, Vector3 origin)
        {
            Rigidbody exampleRigidBody = new Rigidbody();
            exampleRigidBody = rigidBody;
            //SetLayerOnAll(exampleRigidBody.gameObject, 16);
            AttackProjectile exampleAP = exampleRigidBody.gameObject.GetComponent<AttackProjectile>();
            SetLayerOnAll(exampleAP.gameObject, 16);
            Rigidbody projectileInstance = Instantiate(exampleRigidBody, rigidBody.position, rigidBody.rotation) as Rigidbody;

            AttackProjectile newAP = projectileInstance.gameObject.GetComponent<AttackProjectile>();

            newAP.hitModifiers.Clear();

            projectileInstance.velocity = projectileInstance.transform.right * speed * -1;
            //projectileInstance.transform.right = shotDirection * -1;
            //newAP.transform.forward = forwardDirection;

            //newAP.birthModifiers.Clear();
            newAP.StartUp();

        }

        private Vector3 findVelocityDirection()
        {
            return rigidBody.velocity.normalized;
            //return new Vector3()
        }
        private void OnCollisionEnter(Collision other)
        {
            if (gameObject.tag == "projectile_no_collide")
            {
                Physics.IgnoreCollision(other.collider, gameObject.GetComponent<Collider>());
            }
            else
            {
                if (other.gameObject.tag == "enemy")
                {

                    IEnemyController enemyManager = other.gameObject.GetComponentInParent<IEnemyController>();
                    HitEnemy(enemyManager);
                    enemyManager.HitByProjectile(this.GetComponent<AttackProjectile>());

                    Despawn();
                }
                Despawn();
            }

        }

        private void OnCollisionExit(Collision other)
        {
            //if (other.gameObject.tag == "enemy" && gameObject.layer == 16)  //Layer 16 = No collisions
            //{
            //    //gameObject.layer = 13;  // Layer 13 = Player projectiles
            //}
        }

        private void HitEnemy(IEnemyController enemy)
        {
            foreach (AttackModifier nextAM in hitModifiers)
            {
                    if (nextAM.is_enabled && nextAM.type == "chain_shot")
                    {
                        FireAnotherProjectileChain(transform.forward, enemy.parentTransform.position);
                    }
            }
        }
        private float TimeOfDeath = 1f;

        private float deathDelay = 0.0f; // In seconds
        private void Despawn()
        {
            if (alive)
            {
                alive = false;
                CollisionAudio.Play();
                TimeOfDeath = Time.time + deathDelay;
                Destroy(gameObject);
            }
            if (Time.time > TimeOfDeath)
            {

            }

        }
        private Vector3 FindMousePointRelativeToPlayer()
        {
            Vector3 mousePointOnFloor = FindMousePointOnFloor();
            mousePointOnFloor = new Vector3(mousePointOnFloor.x,
                                            mousePointOnFloor.y,
                                            mousePointOnFloor.z) - player.transform.position;
            return mousePointOnFloor;
        }

        private Vector3 FindMousePointOnFloor()
        {
            Vector3 mousePointOnFloor = new Vector3(0, 0, 0);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.tag == "ground")
                {
                    mousePointOnFloor = hit.point;
                }
            }
            return mousePointOnFloor;
        }

    }
}