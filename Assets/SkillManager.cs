using System.Collections;
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
        public AbstractSkillDetail info;

        public AudioSource skillAudio;
        private GameObject player;
        private PlayerManager playerManager;

        private CardManager cardManager;
        private GameObject blinkAnimation;
        private Animator blinkAnim;

        // Start is called before the first frame update
        void Start()
        {
            blinkAnimation = GameObject.Find("BlinkAnimation");
            cardManager = gameObject.GetComponent<CardManager>();
            player = GameObject.Find("Player");
            playerManager = player.GetComponent<PlayerManager>();
            blinkAnim = blinkAnimation.GetComponent<Animator>();
        }

        public void ExecuteSkill(CombatManager combatManager)
        {
            if (info.requiresTarget == SkillTargets.Attack)
            {
                AddAttackModifier(combatManager);
            }
            else if (info.requiresTarget == SkillTargets.Skill)
            {
                AddSkillModifier(combatManager);
            }
            else if (info.requiresTarget == SkillTargets.Ground)
            {
                SkillsTargetGround();
            }
            else {
                SkillsTargetPermanent(combatManager);
            }

        }

        private void AddSkillModifier(CombatManager combatManager) {
            if (info.type == SkillTypes.Duration)
            {
                info.Execute(combatManager);
            }
        }

        private void SkillsTargetGround()
        {
            if (info.type == SkillTypes.Glacier)
            {
                SpawnGlacier();
            }
            else if (info.type == SkillTypes.Blink)
            {
                Blink();
            }
        }


        public void LoadSprite()
        {
            cardManager.LoadSprite();
        }


        private void SkillsTargetPermanent(CombatManager combatManager)
        {
            if (info.type == "dodge_roll")
            {
                DodgeRoll(false);
            }
        }

        private void AddAttackModifier(CombatManager combatManager)
        {
            if (info.type == SkillTypes.DodgeRoll)
            {
                DodgeRoll(true);
            }
            else {
                info.Execute(combatManager);
            }
        }


        private void Blink()
        {
            player.transform.position = FindMousePointRelativeToPlayer();

            blinkAnim.Play("Arive");

            blinkAnimation.transform.position = player.transform.position;
            //blinkAnim.SetBool("Start", false);
        }

        private void SpawnGlacier()
        {
            //combatManager.StartPlacingTerrain(cardManager);
            //AttackModifier spawnGlacier = MainCanvas.AddComponent<AttackModifier>();
            //spawnGlacier.duration = duration;
            //spawnGlacier.magnitude = magnitude;
            //spawnGlacier.type = type;
            //spawnGlacier.icon_name = skill_sprite_name + "Icon";
            ////doubleDuration.transform.SetParent(MainCanvas.transform);
            //combatManager.addAttackModifier(spawnGlacier, cardManager);

        }

        void DodgeRoll(bool shouldDiscardCard)
        {
            playerManager.DodgeRoll();
            cardManager.DiscardCard(shouldDiscardCard);
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