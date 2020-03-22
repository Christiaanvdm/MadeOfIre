using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Complete
{
    public class EnemyManager2D : MonoBehaviour
    {
        private CombatManager combatManager;
        private Renderer healthOrbRenderer;

        private float maximumHealth = 30f;
        private float minimumHealth = 0f;
        private float currentHealth = 30f;
        public string context;
        public Rigidbody projectile;                   // Prefab of the projectile.
        public Transform fireTransform;           // A child of the enemy where the shells are spawned.
        private EnemyManager2D enemyManager;
        public bool isAlive { get { return alive; } }
        public string enemy_name;
        public string type;
        public string description;
        private bool alive = true;
        private Rigidbody rigidBody;
        private EnemyAI2D enemyAI;
        public List<AttackModifier> currentDebufs = new List<AttackModifier>();
        private Collider body;
        private RoomManager roomManager;
        private NavMeshAgent navMeshAgent;
        // Start is called before the first frame update
        void Start()
        {
            rigidBody = gameObject.GetComponent<Rigidbody>();
            roomManager = transform.parent.gameObject.GetComponent<RoomManager>();
            combatManager = GameObject.Find("SceneManager").GetComponent<CombatManager>();
            enemyAI = gameObject.GetComponent<EnemyAI2D>();
            if (!enemyAI) {
                enemyAI = gameObject.GetComponent<EnemyAITypeA>();
            }
            //if (!enemyAI)
            //{
            //    enemyAI = gameObject.GetComponent<Ene>
            //}
            enemyManager = gameObject.GetComponent<EnemyManager2D>();
            navMeshAgent = gameObject.GetComponent<NavMeshAgent>();

            //body = transform.Find("Body").gameObject.GetComponent<Collider>();
        }




        public void HitByProjectile(AttackProjectile projectile)
        {
            transform.position += projectile.rigidBody.velocity.normalized * projectile.knockback;
            var recoilCoroutine = Recoil(2f);
            StartCoroutine(recoilCoroutine);
            currentHealth -= projectile.damage;
            if (currentHealth <= 0)
            {
                death();
            }
            foreach (AttackModifier nextAM in projectile.enemyModifiers)
            {
                AttackModifier newAM = new AttackModifier();
                newAM = nextAM;
                
                addDebuff(newAM);
                
            }
            //a
        }

        IEnumerator Recoil(float duration)
        {
            float initialSpeed = navMeshAgent.speed;
            navMeshAgent.speed = initialSpeed * 0.5f;
            yield return new WaitForSeconds(duration);
            navMeshAgent.speed = initialSpeed;
            yield return null;

        }




        public void addDebuff(AttackModifier amToAdd)
        {
            if (amToAdd.type == "half_speed")
            {
                AttackModifier newAM = gameObject.AddComponent<AttackModifier>();
                newAM.type = amToAdd.type;
                newAM.duration = amToAdd.duration;
                newAM.context = "debuff";
                newAM.magnitude = amToAdd.magnitude;
                navMeshAgent.speed = navMeshAgent.speed * newAM.magnitude;
                currentDebufs.Add(newAM);
                newAM.transform.SetParent(transform); 
                newAM.startTimer(amToAdd, enemyManager);
            }
        }

        public void removeDebuff(AttackModifier amToRemove)
        {
            if (amToRemove.type == "half_speed")
            {

                navMeshAgent.speed = navMeshAgent.speed * 2f;
                currentDebufs.Remove(amToRemove);
                //Destroy(amToRemove);
            }
        }


        private void FixedUpdate()
        {
         
        }
        // Update is called once per frame
        void Update()
        {
            
        }

        private void UpdateHealthDisplay()
        {
            //float fRedValue = (currentHealth / (maximumHealth - minimumHealth));
            //Color newColor = new Color(fRedValue, 0, 0);
            ////Find the Specular shader and change its Color to red
            //healthOrbRenderer.material.shader = Shader.Find("Standard");
            //healthOrbRenderer.material.SetColor("_Color", newColor);
        }

        private void OnMouseDown()
        {

        }

        private void OnMouseEnter()
        {

        }

        private void OnMouseExit()
        {

        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "projectile_no_collide")
            {
                Physics.IgnoreCollision(collision.collider, body);
                //Physics.IgnoreCollision(collision.collider, gameObject.GetComponent<Collider>());
            }
        }

        private void death()
        {
            if (alive)
            {

                gameObject.layer = 16;
                alive = false;
                enemyAI.Death();
                roomManager.enemyCount -= 1;
            }
        }

      

        public void OnCardMouseEnter()
        {
            EnableHighlightSelected();
        }

        public void OnCardMouseExit()
        {
            DisableHighlightSelected();
        }

        public void EnableHighlightSelected()
        {
            if (combatManager.isDraggingACard)
            {
                foreach (Transform child in transform)
                {
                    if (child.tag == "glow")
                    {
                        child.gameObject.SetActive(true);
                    }
                }
            }
        }

        public void DisableHighlightSelected()
        {
            foreach (Transform child in transform)
            {
                if (child.tag == "glow")
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        public void fireShell(float projectileSpeed)
        {
            if (alive)
            {
                //Rigidbody shellInstance = (
                //    Instantiate(projectile, fireTransform.position, fireTransform.rotation) as Rigidbody);
                //// Set the shell's velocity to the launch force in the fire position's forward direction.
                //shellInstance.transform.forward = gameObject.transform.forward;
                //shellInstance.velocity = projectileSpeed * gameObject.transform.forward;
                //shellInstance.gameObject.SetActive(true);
            }
        }

    }
}
