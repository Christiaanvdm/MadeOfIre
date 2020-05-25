using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Complete
{
    public class ModifierObject : MonoBehaviour
    {
        public AbstractSkillDetail info;
        public IEnemyController enemy;
        public bool start = false;
        public GameObject skillCooldownClone;
    }
}