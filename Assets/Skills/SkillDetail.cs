using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Complete
{

    [System.Serializable]
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

    [System.Serializable]
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

    [System.Serializable]
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
}
