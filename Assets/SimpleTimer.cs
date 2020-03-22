using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

using UnityEngine;
using UnityEngine.UI;

namespace Complete
{
    public class SimpleTimer : MonoBehaviour
    {
        public int modifier_count = 0;
        public int id;
        public string type;
        public float magnitude;
        public float duration;
        public string icon_name;

        public CombatManager creator;
        public EnemyManager2D enemy;
        public float targetTime = 60.0f;
        private Object parent;
        public bool start = false;
        public AttackModifier attack_modifier;
        private float endTime;
        private bool cooldownStart = false;
        private Image imageCooldown;
        public GameObject skillCooldownClone;
        public event System.Action OnUpdate;
        public bool is_enabled = true;
        public string context;

      
        private void Start()
        {

        }
        public void ResetTimer()
        {
            start = false;
            creator = null;
            enemy = null;
            //skillCooldownClone = null;


        }
        public void startTimer(AttackModifier attackModifier, EnemyManager2D new_enemy = null, CombatManager owner = null)
        {
          
            if (!start)
            {
               
                endTime = Time.time + attackModifier.duration;
                attack_modifier = attackModifier;
                if (owner)
                {
                    creator = owner;
                    imageCooldown = skillCooldownClone.GetComponent<Image>();
                }
                else if (new_enemy)
                {
                    enemy = new_enemy;
                }
                start = true;
                cooldownStart = true;
            }

        }
    
        private void EnsureDeath()
        {

        }
            



        public void UpdateEvent()
        {
                if (start)
            {
                if (cooldownStart && imageCooldown)
                {
                    imageCooldown.fillAmount -= 1 / duration * Time.deltaTime;
                }
            }
        }




        public void Update()
        {
            if (start)
            {
                if (Time.time > endTime)
                {
                    timerEnded();
                }
                UpdateEvent();
            }

        }

        void timerEnded()
        {
            start = false;
            if (attack_modifier.type == "double_duration")
            {
                creator.removeSkillModifier(attack_modifier);
            }
            else if ((attack_modifier.type == "half_speed") && (enemy))
            {
                enemy.removeDebuff(attack_modifier);
            }
            else if ((attack_modifier.type == "half_speed") && (creator))
            {
                creator.removeAttackModifier(attack_modifier);
            }
            else  
            {
                creator.removeAttackModifier(attack_modifier);
            }

        }




    }
}