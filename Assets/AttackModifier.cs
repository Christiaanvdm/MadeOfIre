using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Complete
{
    public class AttackModifier : MonoBehaviour
    {

        public int modifier_count = 0;
        public int id;
        public string type;
        public float magnitude;
        public float duration;
        public string icon_name;

        public CombatManager creator;
        public IEnemyController enemy;
        public float targetTime = 60.0f;

        public bool start = false;

        public AttackModifier attack_modifier;
        private float endTime;
        private bool cooldownStart = false;
        private Image imageCooldown;
        public GameObject skillCooldownClone;
        public event System.Action OnUpdate;
        public bool is_enabled = true;
        public string context;

        public void setup(string modifier_type, float modifier_magnitude, string icon_name = "")
        {
            type = modifier_type;
            magnitude = modifier_magnitude;
        }

        public AttackModifierBasic CreateAttackModifierBasic() {
            var amb = new AttackModifierBasic();

            amb.context = this.context;
            amb.modifier_count = this.modifier_count;
            amb.type = this.type;
            amb.id = this.id;
            amb.magnitude = this.magnitude;
            amb.icon_name = this.icon_name;
            amb.duration = this.duration;
            amb.enabled = this.enabled;

            return amb;
        }
    }

    public class AttackModifierBasic {
        public int modifier_count = 0;
        public int id;
        public string type;
        public float magnitude;
        public float duration;
        public string icon_name;
        public bool enabled = true;
        public string context;
    }
}