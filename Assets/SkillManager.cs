using System.Collections;
using System.Collections.Generic;

using UnityEngine;
namespace Complete
{
    [System.Serializable]
    public class PlayerState
    {
        public List<SkillDetail> current_deck = new List<SkillDetail>();

        public SkillDetail getRandomCard()
        {
            int value = Mathf.RoundToInt(Random.value * (current_deck.Count - 1f));
            return current_deck[value];
        }
    }

    [System.Serializable]
    public class SkillDetail
    {
        public string requiresTarget = "None";
        public float duration = 2f;
        public float magnitude = 2f;
        public string type = "damage";
        public float cooldown = 1.5f;
        public float projectileSpeed = 10f;
        public float projectileSize = 1f;
        public float projectileDamage;
        public string skill_name;
        public string description;

        public string skill_sprite_name;
    }

    public class SkillManager : MonoBehaviour
    {
        public float duration = 2f;
        public float magnitude = 2f;
        public string type = "damage";
        public float cooldown = 1.5f;
        public float projectileSpeed = 10f;
        public float projectileSize = 1f;
        public float projectileDamage;
        public string skill_name;
        public string description;
        public string skill_sprite_name;


        public AudioSource skillAudio;
        private GameObject player;
        private PlayerManager playerManager;
        private EnemyManager enemy;
        private CombatManager combatManager;
        private Renderer beamRenderer;
        private GameObject beam;
        private GameObject Projectile;


        private Rigidbody projectileRigidbody;
        private Transform originTransform;           // Where the projectile is spawned.
        public string requiresTarget = "None";
        private CardManager cardManager;
        private SkillManager targetSkillManager;
        private GameObject blinkAnimation;
        private GameObject MainCanvas;
        private Animator blinkAnim;

        // Start is called before the first frame update
        void Start()
        {
            combatManager = GameObject.Find("SceneManager").GetComponent<CombatManager>();
            blinkAnimation = GameObject.Find("BlinkAnimation");
            cardManager = gameObject.GetComponent<CardManager>();
            player = GameObject.Find("Player");
            playerManager = player.GetComponent<PlayerManager>();
            beam = GameObject.Find("BeamAttack");
            Projectile = GameObject.Find("AttackProjectile");
            projectileRigidbody = Projectile.gameObject.GetComponent<Rigidbody>();
            originTransform = player.transform;
            MainCanvas = GameObject.Find("HUDUICanvas");
            blinkAnim = blinkAnimation.GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void Attack()
        {
          
            Vector3 shotDirection = (FindMousePointRelativeToPlayer() - player.transform.position).normalized;
            Rigidbody projectileInstance = Instantiate(projectileRigidbody, originTransform.position + new Vector3(0, 0.1f, -0.2f), originTransform.rotation) as Rigidbody;
            projectileInstance.velocity = shotDirection * projectileSpeed;
            cardManager.cooldown = 1.5f;
            projectileInstance.transform.up = new Vector3(0, 1, 0);
            AttackProjectile nextAP = projectileInstance.GetComponent<AttackProjectile>();
            nextAP.speed = projectileSpeed;
            combatManager.ModifyProjectile(ref nextAP);
            nextAP.StartUp();
            cardManager.DiscardCard(false);

        }

        private void ModifySkill()
        {
            //foreach (AttackModifier nextModifier in combatManager.SkillModifierList)
            //{
            //    if (nextModifier.type == "double_duration")
            //    {
            //        duration = duration * nextModifier.magnitude;
            //        combatManager.removeAttackModifier(nextModifier);
            //    }
            //}
        }

        public void ExecuteSkill(EnemyManager enemyManager = null, SkillManager TargetSkillManager = null)
        {
            if (combatManager.SkillModifierList.Count > 0)
            {
                ModifySkill();
            }
            if (requiresTarget == "Enemy")
            {
                SkillsTargetEnemy(enemyManager);
            }
            else if (requiresTarget == "None")
            {
                SkillsTargetNone();
            }
            else if (requiresTarget == "Card")
            {
                targetSkillManager = TargetSkillManager;
            }
            else if (requiresTarget == "Permanent")
            {
                SkillsTargetPermanent();
            }
            else if (requiresTarget == "Ground")
            {
                SkillsTargetGround();
            }
        }

        private void SkillsTargetGround()
        {
            if (type == "spawn_glacier")
            {
                SpawnGlacier();
            }
        }


        public void LoadSprite()
        {
            cardManager.LoadSprite();
        }


        private void SkillsTargetPermanent()
        {
            if (type == "attack")
            {
                Attack();
            }
            else if (type == "dodge_roll")
            {
                DodgeRoll(false);
            }
        }

        private void SkillsTargetEnemy(EnemyManager enemyManager)
        {
            enemy = enemyManager;
            if (enemy != null)
            {
                if (type == "beam_attack")
                {
                    first_beam = true;
                    BeamAttack();

                }
            }
        }

        private void SkillsTargetNone()
        {
            if (type == "dodge_roll")
            {
                DodgeRoll(true);
            }
            else if (type == "double_size")
            {
                DoubleSize();
            }
            else if (type == "double_damage")
            {
                DoubleDamage();
            }
            else if (type == "double_duration")
            {
                DoubleDuration();
            }
            else if (type == "half_speed")
            {
                EnemyHalfSpeed();
            }
            else if (type == "split_shot")
            {
                SplitShot();
            }
            else if (type == "chain_shot")
            {
                ChainShot();
            }
            else if (type == "spawn_glacier")
            {           
                    SpawnGlacier();
            }
            else if (type == "blink")
            {
               
                    Blink();
 
            }
        }

        private void Blink()
        {
            player.transform.position = FindMousePointRelativeToPlayer();

            blinkAnim.Play("Arive");

            blinkAnimation.transform.position = player.transform.position;
            //blinkAnim.SetBool("Start", false);
            cardManager.DiscardCard();
        }

        private void SpawnGlacier()
        {
            combatManager.StartPlacingTerrain(cardManager);
            //AttackModifier spawnGlacier = MainCanvas.AddComponent<AttackModifier>();
            //spawnGlacier.duration = duration;
            //spawnGlacier.magnitude = magnitude;
            //spawnGlacier.type = type;
            //spawnGlacier.icon_name = skill_sprite_name + "Icon";
            ////doubleDuration.transform.SetParent(MainCanvas.transform);
            //combatManager.addAttackModifier(spawnGlacier, cardManager);

        }


        private void ChainShot()
        {
            AttackModifier chainShot = MainCanvas.AddComponent<AttackModifier>();
            chainShot.duration = duration;
            chainShot.magnitude = magnitude;
            chainShot.type = type;
            chainShot.icon_name = skill_sprite_name + "Icon";
            //doubleDuration.transform.SetParent(MainCanvas.transform);
            combatManager.addAttackModifier(chainShot, cardManager);
        }


        private void SplitShot()
        {
            AttackModifier splitShot = MainCanvas.AddComponent<AttackModifier>();
            splitShot.duration = duration;
            splitShot.magnitude = magnitude;
            splitShot.type = type;
            splitShot.icon_name = skill_sprite_name + "Icon";
            //doubleDuration.transform.SetParent(MainCanvas.transform);
            combatManager.addAttackModifier(splitShot, cardManager);
        }

        private void EnemyHalfSpeed()
        {
            CreateNova();
            AttackModifier enemyHalfSpeed = MainCanvas.AddComponent<AttackModifier>();
            enemyHalfSpeed.duration = duration;
            enemyHalfSpeed.magnitude = magnitude;
            enemyHalfSpeed.type = type;
            enemyHalfSpeed.icon_name = skill_sprite_name + "Icon";
            //doubleDuration.transform.SetParent(MainCanvas.transform);
            combatManager.addAttackModifier(enemyHalfSpeed, cardManager);
        }

      
        private void DoubleDuration()
        {
            AttackModifier doubleDuration = MainCanvas.AddComponent<AttackModifier>();
            doubleDuration.duration = duration;
            doubleDuration.magnitude = magnitude;
            doubleDuration.type = type;
            doubleDuration.icon_name = skill_sprite_name + "Icon";
            //doubleDuration.transform.SetParent(MainCanvas.transform);
            combatManager.addSkillModifier(doubleDuration, cardManager);
        }




        private void DoubleDamage()
        {
            AttackModifier doubleDamage = MainCanvas.AddComponent<AttackModifier>();

            doubleDamage.duration = duration;
            doubleDamage.magnitude = magnitude;
            doubleDamage.type = type;
            doubleDamage.icon_name = skill_sprite_name + "Icon";

            combatManager.addAttackModifier(doubleDamage, cardManager);
        }

        void DoubleSize()
        {
            AttackModifier doubleSize = MainCanvas.AddComponent<AttackModifier>();
            doubleSize.duration = duration;
            doubleSize.magnitude = magnitude;
            doubleSize.type = type;
            doubleSize.icon_name = skill_sprite_name + "Icon";
            //doubleSize.transform.SetParent(MainCanvas.transform);
            combatManager.addAttackModifier(doubleSize, cardManager);
        }
        void DodgeRoll(bool shouldDiscardCard)
        {
            playerManager.DodgeRoll();
            cardManager.DiscardCard(shouldDiscardCard);
        }


        private void CreateNova()
        {
            GameObject newNova = Instantiate(Resources.Load("Nova")) as GameObject;
        }



        void BeamAttack()
        {
            //CreateBeam();
            //enemy.TakeDamage(magnitude);
        }
        bool first_beam = true;

        void CreateBeam()
        {
            if (first_beam)
            {
                skillAudio.Play();
                Vector3 enemyPosition = enemy.gameObject.transform.position;
                Vector3 playerPosition = player.gameObject.transform.position;

                LineRenderer lr = beam.GetComponent<LineRenderer>();
                beam.transform.position = playerPosition;
                Vector3[] positions = new Vector3[2];
                positions[0] = new Vector3(0.0f, 0.5f, 0.0f);
                positions[1] = enemyPosition - playerPosition;
                lr.positionCount = positions.Length;
                lr.SetPositions(positions);
                first_beam = false;
            }
            else
            {

                beamRenderer = beam.gameObject.GetComponent<Renderer>();
                DestroyBeam();

            }
        }
        float opacityValue = 0.5f;
        private void DestroyBeam()
        {
            Color newColor = new Color(1, 0, 0, opacityValue);
            //Find the Specular shader and change its Color to red
            beamRenderer.material.shader = Shader.Find("Standard");
            beamRenderer.material.SetColor("_Color", newColor);
            opacityValue = opacityValue - 0.0001f;
            if (opacityValue < 0)
            {
                gameObject.SetActive(false);
            }
        }

        Vector3 FindMousePointRelativeToPlayer()
        {
            Vector3 mousePointOnFloor = FindMousePointOnFloor();
            mousePointOnFloor = new Vector3(mousePointOnFloor.x,
                                            player.transform.position.y,
                                            mousePointOnFloor.z);
            return mousePointOnFloor;
        }

        Vector3 FindMousePointOnFloor()
        {
            Vector3 mousePointOnFloor = new Vector3(0, 0, 0);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            print(Input.mousePosition);
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

        public void SkillWithoutEnemy()
        {
            ExecuteSkill();

        }




    }


}