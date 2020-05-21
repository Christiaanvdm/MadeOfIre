﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;
namespace Complete
{

    public static class SkillTypes
    {
        public const string Damage = "double_damage";
        public const string Size = "double_size";
        public const string Blink = "blink";
        public const string Glacier = "spawn_glacier";
        public const string SplitShot = "split_shot";
        public const string Slow = "half_speed";
        public const string Duration = "double_duration";
        public const string Bounce = "bounce";
        public const string DodgeRoll = "dodge_roll";
        public const string Chain = "chain_shot";
    }

    public static class SkillTargets
    {
        public const string Ground = "ground";
        public const string Attack = "attack";
        public const string Skill = "skill";
    }

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

    public class SkillManager : MonoBehaviour
    {
        public SkillDetail skillDetail;

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

            originTransform = player.transform;
            MainCanvas = GameObject.Find("HUDUICanvas");
            blinkAnim = blinkAnimation.GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Attack()
        {

            Vector3 shotDirection = (FindMousePointRelativeToPlayer() - player.transform.position).normalized;
            var projectileRigidbody = Resources.Load<Rigidbody>("AttackProjectile");
            Rigidbody projectileInstance = Instantiate(projectileRigidbody, originTransform.position + new Vector3(0, 0.1f, -0.2f), originTransform.rotation) as Rigidbody;
            projectileInstance.velocity = shotDirection * projectileSpeed;
            cardManager.cooldown = 1.5f;
            //projectileInstance.transform.up = new Vector3(0, 1, 0);
            AttackProjectile nextAP = projectileInstance.GetComponent<AttackProjectile>();
            nextAP.speed = projectileSpeed;
            combatManager.ModifyProjectile(ref nextAP);
            nextAP.StartUp();
            cardManager.DiscardCard(false);

        }

        public void ExecuteSkill()
        {
            if (skillDetail.requiresTarget == SkillTargets.Attack)
            {
                AddAttackModifier();
            }
            else if (requiresTarget == SkillTargets.Skill)
            {
                AddSkillModifier();
            }
            else if (requiresTarget == SkillTargets.Ground)
            {
                SkillsTargetGround();
            }
            else {
                SkillsTargetPermanent();
            }
        }

        private void AddSkillModifier() {
            if (type == SkillTypes.Duration)
            {
                DoubleDuration();
            }
        }

        private void SkillsTargetGround()
        {
            if (type == SkillTypes.Glacier)
            {
                SpawnGlacier();
            }
            else if (type == SkillTypes.Blink)
            {
                Blink();
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

        private void AddAttackModifier()
        {
            if (type == SkillTypes.DodgeRoll)
            {
                DodgeRoll(true);
            }
            else if (type == SkillTypes.Slow)
            {
                EnemyHalfSpeed();
            }
            else {
                skillDetail.Execute(combatManager, cardManager);
            }
        }


        private void Bounce() {
            AttackModifier bounceShot = MainCanvas.AddComponent<AttackModifier>();
            bounceShot.duration = duration;
            bounceShot.magnitude = magnitude;
            bounceShot.type = type;
            bounceShot.icon_name = skill_sprite_name + "Icon";
            combatManager.addAttackModifier(bounceShot, cardManager);
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
            //GameObject newNova = Instantiate(Resources.Load("Nova")) as GameObject;
        }



        void BeamAttack()
        {
            //CreateBeam();
            //enemy.TakeDamage(magnitude);
        }
        bool first_beam = true;

        //void CreateBeam()
        //{
        //    if (first_beam)
        //    {
        //        skillAudio.Play();
        //        Vector3 enemyPosition = enemy.gameObject.transform.position;
        //        Vector3 playerPosition = player.gameObject.transform.position;

        //        LineRenderer lr = beam.GetComponent<LineRenderer>();
        //        beam.transform.position = playerPosition;
        //        Vector3[] positions = new Vector3[2];
        //        positions[0] = new Vector3(0.0f, 0.5f, 0.0f);
        //        positions[1] = enemyPosition - playerPosition;
        //        lr.positionCount = positions.Length;
        //        lr.SetPositions(positions);
        //        first_beam = false;
        //    }
        //    else
        //    {

        //        beamRenderer = beam.gameObject.GetComponent<Renderer>();
        //        DestroyBeam();

        //    }
        //}
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
    }
}