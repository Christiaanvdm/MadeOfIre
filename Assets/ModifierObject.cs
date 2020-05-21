using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Complete
{
    public class ModifierObject : MonoBehaviour
    {
        public ModifierInfo info = new ModifierInfo();
        public CombatManager creator;
        public IEnemyController enemy;
        public bool start = false;
        public GameObject skillCooldownClone;
    }

    public class ModifierInfo {
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