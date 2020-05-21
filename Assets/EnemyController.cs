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
        void addDebuff(ModifierInfo attackModifier);
        void removeDebuff(ModifierInfo attackModifier);
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
        private List<ModifierInfo> debuffList;

        public float bulletVelocity { get; set; }
        public float health { get; set; }

        // Start is called before the first frame update
        void Start()
        {
            debuffList = new List<ModifierInfo>();
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

        private List<ModifierInfo> GroupAttackModifiers(List<ModifierObject> attackModifiers)
        {
            var groupedModifiersList = new List<ModifierInfo>();
            var groupedBirthModifiers = attackModifiers.GroupBy(x => x.info.type).ToList();
            ModifierInfo nextAm = new ModifierInfo();
            foreach (var group in groupedBirthModifiers)
            {
                bool first = true;
                foreach (var item in group)
                {
                    if (item == null)
                        continue;

                    if (first)
                    {
                        nextAm = new ModifierInfo();
                        first = false;
                    };

                    nextAm = item.info;
                }
                groupedModifiersList.Add(nextAm);
            }

            return groupedModifiersList;
        }

        public Transform parentTransform => transform;

        protected void applyDebuff(ModifierInfo debuff)
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

        public virtual void removeDebuff(ModifierInfo debuff)
        {
            if (debuff.type == SkillTypes.Slow)
            {
                navMeshAgent.speed = navMeshAgent.speed * debuff.magnitude;
            }

            debuffList.Remove(debuff);
        }

        public virtual void addDebuff(ModifierInfo attackModifier)
        {
            StartCoroutine(ApplyAndRemoveDebuff(attackModifier));
        }


        IEnumerator ApplyAndRemoveDebuff(ModifierInfo am)
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