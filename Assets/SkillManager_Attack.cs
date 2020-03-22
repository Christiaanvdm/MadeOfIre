using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Complete { 
    public class SkillManager_Attack : MonoBehaviour
    {
        public string skill_name;
        public string type;
        public Rigidbody projectileRigidbody;
        public Transform originTransform;           // Where the projectile is spawned.
        public int value;
        public string description;
        public AudioSource skillAudio;
        private GameObject player;
        private EnemyManager enemy;
        private Renderer beamRenderer;
        private GameObject Projectile;
        private float projectileSpeed = 1f;

        // Start is called before the first frame update
        void Start()
        {
            player = GameObject.FindWithTag("player");
            Projectile = GameObject.Find("AttackProjectile");
        }

        // Update is called once per frame
        void Update()
        {

        }
        Vector3 FindMousePointRelativeToPlayer()
        {
            Vector3 mousePointOnFloor = FindMousePointOnFloor();
            mousePointOnFloor = new Vector3(mousePointOnFloor.x, mousePointOnFloor.y + player.transform.position.y, mousePointOnFloor.z);
            return mousePointOnFloor;

        }
        Vector3 FindMousePointOnFloor()
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

        public void ExecuteSkill(EnemyManager enemyManager)
        {
            enemy = enemyManager;
            Attack();

        }

        void Attack()
        {
            Vector3 shotDirection = (FindMousePointRelativeToPlayer() - player.transform.position).normalized;
            // Generate projectile
            Rigidbody projectileInstance =
                Instantiate(projectileRigidbody, originTransform.position, originTransform.rotation) as Rigidbody;
            projectileInstance.velocity = shotDirection * projectileSpeed;
           
        }

     
        //void BeamAttack()
        //{
        //    CreateBeam();
        //    enemy.TakeDamage(value);
        //}
        //bool first_beam = true;

        //void CreateBeam()
        //{

        //    if (first_beam)
        //    {
        //        skillAudio.Play();
        //        Vector3 enemyPosition = enemy.gameObject.transform.position;
        //        Vector3 playerPosition = player.gameObject.transform.position;

        //        LineRenderer lr = Projectile.GetComponent<LineRenderer>();
        //        Projectile.transform.position = playerPosition;
        //        Vector3[] positions = new Vector3[2];
        //        positions[0] = new Vector3(0.0f, 0.5f, 0.0f);
        //        positions[1] = enemyPosition - playerPosition;
        //        lr.positionCount = positions.Length;
        //        lr.SetPositions(positions);
        //        first_beam = false;
        //    }
        //    else
        //    {

        //        beamRenderer = Projectile.gameObject.GetComponent<Renderer>();
        //        DestroyBeam();

        //    }
        //}
        //float opacityValue = 0.5f;
        //private void DestroyBeam()
        //{

        //    Color newColor = new Color(1, 0, 0, opacityValue);
        //    //Find the Specular shader and change its Color to red
        //    beamRenderer.material.shader = Shader.Find("Standard");
        //    beamRenderer.material.SetColor("_Color", newColor);
        //    opacityValue = opacityValue - 0.0001f;
        //    if (opacityValue < 0)
        //    {
        //        gameObject.SetActive(false);
        //    }
        //}


    }
}
