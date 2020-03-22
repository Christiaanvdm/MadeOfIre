using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Complete
{
    public class EnemyManager : MonoBehaviour
    {
        private CombatManager combatManager;
        private Renderer healthOrbRenderer;

        private float maximumHealth = 30f;
        private float minimumHealth = 0f;
        private float currentHealth = 30f;
    
        public Rigidbody projectile;                   // Prefab of the projectile.
        public Transform fireTransform;           // A child of the enemy where the shells are spawned.
       
        public bool isAlive { get { return alive; } }
        public string enemy_name;
        public string type;
        public string description;
        private bool alive = true;
        private Rigidbody rigidBody;
        // Start is called before the first frame update
        void Start()
        {
            rigidBody = gameObject.GetComponent<Rigidbody>();
            foreach (Transform child in transform)
            {
                if (child.tag == "health_orb")
                {
                    healthOrbRenderer = child.GetComponent<Renderer>();
                }
            }
            combatManager = GameObject.Find("SceneManager").GetComponent<CombatManager>();

        }

        // Update is called once per frame
        void Update()
        {
       
        }

        private void UpdateHealthDisplay()
        {
            float fRedValue = (currentHealth / (maximumHealth - minimumHealth));
            Color newColor = new Color(fRedValue, 0, 0);
            //Find the Specular shader and change its Color to red
            healthOrbRenderer.material.shader = Shader.Find("Standard");
            healthOrbRenderer.material.SetColor("_Color", newColor);
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

        private void death()
        {
            alive = false;
            rigidBody.useGravity = true;
            rigidBody.constraints = RigidbodyConstraints.None;
            rigidBody.angularDrag = 0.05f;
        }

        public void TakeDamage(float damage)
        {
            
            currentHealth = currentHealth - damage;
            if (currentHealth < 0)
            {
                currentHealth = 0;
                death();
            }
            UpdateHealthDisplay();
            print("Enemy " + enemy_name + " took " + damage.ToString() + " points of damage. " +
            	"Current health: " + currentHealth.ToString());

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
                    if (child.tag == "glow") { 
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
            //if (alive)
            //{
            //    Rigidbody shellInstance = (
            //        Instantiate(projectile, fireTransform.position, fireTransform.rotation) as Rigidbody);
            //    // Set the shell's velocity to the launch force in the fire position's forward direction.
            //    shellInstance.transform.forward = gameObject.transform.forward;
            //    shellInstance.velocity = projectileSpeed * gameObject.transform.forward;
            //    shellInstance.gameObject.SetActive(true);
            //}
        }

    }
}
