﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Complete
{
    public class AttackProjectile : MonoBehaviour
    {
        public float damage = 1f;
        public float scale = 1f;
        public float speed = 0f;
        public float knockback = 0.1f;
        public float daze_duration = 0.5f;
        public float cooldown;
        private bool alive = true;

        public int bounces = 0;
        public Rigidbody rigidBody;
        private GameObject sprite;
        public AudioSource CollisionAudio;
        public GameObject player;
        private Quaternion originalRotation;
        public Projectile projectile;


        private GameObject childSprite;
        private GameObject exampleProjectile;
        private GameObject exampleSprite;


        public List<ModifierObject> enemyModifiers = new List<ModifierObject>();
        public List<ModifierObject> birthModifiers = new List<ModifierObject>();
        public List<ModifierObject> allAttackModifiers = new List<ModifierObject>();
        public List<ModifierObject> hitModifiers = new List<ModifierObject>();
        private void resetSpriteOrientation()
        {
            childSprite.transform.SetPositionAndRotation(childSprite.transform.position, exampleSprite.transform.rotation);
        }

        public void AddBounces(int number)
        {
            bounces += number;
        }


        // Start is called before the first frame update
        void Start()
        {
            originalRotation = gameObject.transform.rotation;
            player = GameObject.Find("Player");
            sprite = gameObject.transform.Find("Sprite").gameObject;


        }

        IEnumerator despawnAfterFixedInterval() {
            while (true) {
                yield return new WaitForSeconds(6f);
                Despawn();
            }
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

        public void AddModifier(ModifierObject newAM)
        {
            if (newAM.info.context == "Birth")
            {
                birthModifiers.Add(newAM);
            }
            else if (newAM.info.context == "Enemy")
            {
                enemyModifiers.Add(newAM);
            }
            else if (newAM.info.context == "Hit")
            {
                hitModifiers.Add(newAM);
            }

        }

        public void StartUp()
        {
            GroupAttackModifiers();
            rigidBody = gameObject.GetComponent<Rigidbody>();
            exampleProjectile = Resources.Load<GameObject>("AttackProjectile");
            childSprite = rigidBody.transform.Find("Sprite").gameObject;
            exampleSprite = exampleProjectile.transform.Find("Sprite").gameObject;
            //renderer = rigidbody.gameObject.GetComponent<Renderer>();
            //resetSpriteOrientation();
            Birth();

        }

        private void GroupAttackModifiers()
        {
            var groupedModifiersList = new List<ModifierObject>();
            var groupedBirthModifiers = birthModifiers.GroupBy(x => x.info.type).ToList();
            ModifierObject nextAm = new ModifierObject();
            foreach (var group in groupedBirthModifiers)
            {
                bool first = true;
                foreach (var item in group)
                {
                    if (item == null)
                        continue;
                    if (first)
                    {
                        nextAm = new ModifierObject();
                        first = false;
                        nextAm.info = item.info;
                    };
                    nextAm.info.magnitude += item.info.magnitude;
                }
                groupedModifiersList.Add(nextAm);
            }

            this.birthModifiers = groupedModifiersList;
        }

        private void Birth()
        {
            Vector3 projectileForward = transform.right * -1;
            foreach (ModifierObject nextAM in birthModifiers)
            {
                if (nextAM.enabled && (nextAM.info.type == "split_shot"))
                {
                    for (int i = 0; i < nextAM.info.magnitude + 1; i++)
                    {
                        float angleAdjustment = 30 + i * (60 / nextAM.info.magnitude + 1);
                        if (angleAdjustment != 0) {
                            FireAnotherProjectile(offsetDirectionByAngle(rigidBody.velocity.normalized, -30 + angleAdjustment), rigidBody.transform.position);
                        }

                    }
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
            GameObject projectileInstance = Instantiate(gameObject, transform.position, transform.rotation) as GameObject;

            //SetLayerOnAll(newAP.gameObject, 16);
            AttackProjectile newAP = projectileInstance.gameObject.GetComponent<AttackProjectile>();
            projectileInstance.GetComponent<Rigidbody>().velocity = shotDirection.normalized * speed;
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
                    if (enemyManager == null)
                        enemyManager = other.gameObject.GetComponent<IEnemyController>();
                    HitEnemy(enemyManager);
                    enemyManager.HitByProjectile(this.GetComponent<AttackProjectile>());

                    Bounce();
                }
                if (other.gameObject.tag == "terrain") {
                    IAttackable target = other.gameObject.GetComponentInParent<IAttackable>();
                    target.HitByProjectile(this.GetComponent<AttackProjectile>());
                }
                Bounce();
            }

        }

        private void Bounce()
        {
            if (bounces > 0)
                bounces -= 1;
            else
                Despawn();
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
            foreach (ModifierObject nextAM in hitModifiers)
            {
                if (nextAM.enabled && nextAM.info.type == "chain_shot")
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