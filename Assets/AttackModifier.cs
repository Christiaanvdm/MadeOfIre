using System.Reflection;

namespace Complete
{
    public class AttackModifier : SimpleTimer
    {
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