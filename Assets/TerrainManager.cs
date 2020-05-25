using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Complete
{
    public class TerrainManager : MonoBehaviour, IAttackable
    {
        public bool collided = false;
        public GameObject collider;
        Animator anim;

        float health = 10f;
        // Start is called before the first frame update
        void Start()
        {
            anim = transform.Find("Sprite").GetComponent<Animator>();
            anim.enabled = false;
            DisableColliders();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ActivateColliders()
        {
            transform.Find("Collider").gameObject.SetActive(true);
        }

        public void DisableColliders()
        {
            transform.Find("Collider").gameObject.SetActive(false);
        }

        public void Spawn()
        {
            ActivateColliders();
            anim.enabled = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            collided = true;
            //Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), other);
        }

        public void HitByProjectile(AttackProjectile projectile)
        {
            health -= projectile.damage;
            if (health <= 0)
                Death();
        }

        private void Death() {
            gameObject.SetActive(false);
        }
    }
}