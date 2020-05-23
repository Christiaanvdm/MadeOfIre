using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Complete
{

    [Serializable]
    public class SkillDetail
    {
        public string requiresTarget;
        public float duration;
        public float magnitude;
        public string type;
        public float cooldown;
        public float projectileDamage;
        public string skill_name;
        public string description;
        public string skill_sprite_name;
        public string class_name;

        public int modifier_count = 0;
        public int id;

        public string icon_name;
        public bool enabled = true;
        public string context;
    }

    public abstract class AbstractSkillDetail : SkillDetail
    {
        public abstract void Execute(CombatManager context);
    }

    public class Debuff : SkillDetail
    {
        public CancellationTokenSource cancellationToken = new CancellationTokenSource();
    }

    [Serializable]
    public class SkillModifier : AbstractSkillDetail
    {
        public override void Execute(CombatManager context)
        {
            ModifierObject am = new ModifierObject();
            am.info.duration = duration;
            am.info.magnitude = magnitude;
            am.info.type = type;
            am.info.icon_name = skill_sprite_name + "Icon";

            context.addSkillModifier(am);
        }
    }

    [Serializable]
    public class AttackModifier : AbstractSkillDetail
    {
        public override void Execute(CombatManager context)
        {
            ModifierObject am = new ModifierObject();
            am.info.duration = duration;
            am.info.magnitude = magnitude;
            am.info.type = type;
            am.info.icon_name = skill_sprite_name + "Icon";

            context.addAttackModifier(am);
        }
    }

    [Serializable]
    public class BlinkSkill : AbstractSkillDetail
    {
        private Animator blinkAnim;
        private GameObject blinkAnimation;
        public override void Execute(CombatManager context)
        {
            blinkAnimation = Resources.Load<GameObject>("Skills/BlinkAnimation");
            blinkAnim = blinkAnimation.GetComponent<Animator>();
            context.player.transform.position = FindMousePointRelativeToPlayerWithPlayerY(context.player);

            blinkAnim.Play("Arive");

            blinkAnimation.transform.position = context.player.transform.position;
        }

        Vector3 FindMousePointRelativeToPlayerWithPlayerY(GameObject player)
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

    [Serializable]
    public class PlaceTerrain : AbstractSkillDetail
    {
        public override void Execute(CombatManager context)
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
    }

    public class DodgeRoll : AbstractSkillDetail {
        public override void Execute(CombatManager context)
        {
            context.playerManager.DodgeRoll();
        }
    }
}
