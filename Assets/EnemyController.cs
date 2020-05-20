using JetBrains.Annotations;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

namespace Complete
{
    public interface IEnemyController
    {
        void HitByProjectile(AttackProjectile projectile);
        Transform parentTransform { get; }
        void addDebuff(AttackModifierBasic attackModifier);
        void removeDebuff(AttackModifierBasic attackModifier);
        void Death();
        void onStart();

        float bulletVelocity { get; set; }
        float health { get; set; }
    }


    public class EnemyController : MonoBehaviour, IEnemyController
    {
        protected Animator anim;
        protected NavMeshAgent navMeshAgent;
        protected Transform colliderTransform;
        protected Transform enemySprite;
        protected Transform player = null;
        protected bool finalDeath = false;
        protected Quaternion initialRotation;
        private List<AttackModifierBasic> debuffList;

        public float bulletVelocity { get; set; }
        public float health { get; set; }

        // Start is called before the first frame update
        void Start()
        {
            debuffList = new List<AttackModifierBasic>();
            anim = transform.Find("Anim").GetComponent<Animator>();
            navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
            colliderTransform = transform.Find("Collider");
            enemySprite = transform.Find("Anim");
            player = GameObject.Find("Player").transform;
            initialRotation = transform.rotation;
            onStart();
        }

        public virtual void onStart() { }
        public virtual void ApplyProjectile(AttackProjectile projectile) {
            health -= projectile.damage;
            if (health < 0)
            {
                Death();
            }
        }
        public virtual void Death() { }

        public void HitByProjectile(AttackProjectile projectile)
        {

            foreach (var am in GroupAttackModifiers(projectile.enemyModifiers))
            {
                addDebuff(am);
            }
            if (health > 0)
            {
                health -= projectile.damage;
            }
            else {
                Death();
            }
        }

        private List<AttackModifierBasic> GroupAttackModifiers(List<AttackModifier> attackModifiers)
        {
            var groupedModifiersList = new List<AttackModifierBasic>();
            var groupedBirthModifiers = attackModifiers.GroupBy(x => x.type).ToList();
            AttackModifierBasic nextAm = new AttackModifierBasic();
            foreach (var group in groupedBirthModifiers)
            {
                bool first = true;
                foreach (var item in group)
                {
                    if (first)
                    {
                        nextAm = new AttackModifierBasic();
                        first = false;
                    };
                    nextAm.duration = item.duration;
                    nextAm.magnitude += item.magnitude;
                    nextAm.icon_name = item.icon_name;
                    nextAm.enabled = item.enabled;
                    nextAm.type = item.type;
                }
                groupedModifiersList.Add(nextAm);
            }

            return groupedModifiersList;
        }

        public Transform parentTransform => transform;

        protected void applyDebuff(AttackModifierBasic debuff)
        {
            var existingDebuff = debuffList.FirstOrDefault(am => am.type == debuff.type);
            if (existingDebuff != null) {
                existingDebuff.enabled = false;
                removeDebuff(existingDebuff);
                debuffList.Remove(existingDebuff);
            }

            if (debuff.type == SkillTypes.Slow) {
                navMeshAgent.speed = navMeshAgent.speed / debuff.magnitude;
            }
            debuffList.Add(debuff);
        }

        public virtual void removeDebuff(AttackModifierBasic debuff)
        {
            if (debuff.type == SkillTypes.Slow)
            {
                navMeshAgent.speed = navMeshAgent.speed * debuff.magnitude;
            }
            debuffList.Remove(debuff);
        }

        public virtual void addDebuff(AttackModifierBasic attackModifier)
        {
            StartCoroutine(ApplyAndRemoveDebuff(attackModifier));
        }


        IEnumerator ApplyAndRemoveDebuff(AttackModifierBasic am)
        {
            for (int i = 0; i < 1; i++)
            {
                applyDebuff(am);
                yield return new WaitForSeconds(am.duration);
            }
            if (am.enabled == true) {
                removeDebuff(am);
            }

            yield break;
        }

    }
}