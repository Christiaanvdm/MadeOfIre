using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Complete
{
    public class ModifierObject : MonoBehaviour
    {
        public SkillDetail info = new SkillDetail();
        public CombatManager creator;
        public IEnemyController enemy;
        public bool start = false;
        public GameObject skillCooldownClone;
    }

    //public class SkillDetail {
    //    public int modifier_count = 0;
    //    public int id;
    //    public string type;
    //    public float magnitude;
    //    public float duration;
    //    public string icon_name;
    //    public bool enabled = true;
    //    public string context;
    //}
}