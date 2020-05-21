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
        public float projectileSpeed;
        public float projectileSize;
        public float projectileDamage;
        public string skill_name;
        public string description;
        public string skill_sprite_name;

        public virtual void Execute(CombatManager context, CardManager card) {
            AttackModifier am = new AttackModifier();
            am.duration = duration;
            am.magnitude = magnitude;
            am.type = type;
            am.icon_name = skill_sprite_name + "Icon";

            context.addAttackModifier(am, card);
        }
    }

    public class DamageSkillDetail : SkillDetail {

    }

}
