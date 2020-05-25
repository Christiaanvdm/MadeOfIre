using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

namespace Complete
{
    public interface IEnemyController: IAttackable
    {

        Transform parentTransform { get; }
        void addDebuff(SkillDetail attackModifier);
        void removeDebuff(Debuff attackModifier);
        void Death();
        void onStart();

        float bulletVelocity { get; set; }
        float health { get; set; }
    }

    public interface IAttackable
    {
        void HitByProjectile(AttackProjectile projectile);
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
        private List<Debuff> debuffList;

        public float bulletVelocity { get; set; }
        public float health { get; set; }

        // Start is called before the first frame update
        void Start()
        {
            debuffList = new List<Debuff>();
            anim = transform.Find("Anim").GetComponent<Animator>();
            navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
            colliderTransform = transform.Find("Collider");
            enemySprite = transform.Find("Anim");
            player = GameObject.Find("Player").transform;
            initialRotation = transform.rotation;
            onStart();
        }

        public virtual void onStart() { }
        public virtual void ApplyProjectile(AttackProjectile projectile)
        {
            health -= projectile.damage;
            if (health < 0)
            {
                Death();
            }
        }
        public virtual void Death() { }

        public void HitByProjectile(AttackProjectile projectile)
        {

            var groupedAttackModifiers = GroupAttackModifiers(projectile.enemyModifiers);
            foreach (var am in groupedAttackModifiers)
            {
                addDebuff(am);
            }
            if (health > 0)
            {
                health -= projectile.damage;
            }
            else
            {
                Death();
            }
        }

        private List<SkillDetail> GroupAttackModifiers(List<ModifierObject> attackModifiers)
        {
            var groupedModifiersList = new List<SkillDetail>();
            var groupedBirthModifiers = attackModifiers.GroupBy(x => x.info.type).ToList();
            SkillDetail nextAm = new SkillDetail();
            foreach (var group in groupedBirthModifiers)
            {
                bool first = true;
                foreach (var item in group)
                {
                    if (item.info == null)
                        continue;

                    if (first)
                    {
                        nextAm = new SkillDetail();
                        nextAm = item.info;

                        first = false;
                    }
                    else {
                        nextAm.magnitude += item.info.magnitude;
                    }
                }
                groupedModifiersList.Add(nextAm);
            }

             return groupedModifiersList;
        }

        public Transform parentTransform => transform;

        protected void applyDebuff(Debuff debuff)
        {
            var existingDebuff = debuffList.FirstOrDefault(am => am.type == debuff.type);
            if (existingDebuff != null)
            {
                debuff.cancellationToken.Cancel();

                debuffList.Remove(existingDebuff);
            }

            if (debuff.type == SkillTypes.Slow)
            {
                navMeshAgent.speed = navMeshAgent.speed / debuff.magnitude;
            }

            debuffList.Add(debuff);
        }

        public virtual void removeDebuff(Debuff debuff)
        {
            if (debuff.type == SkillTypes.Slow)
            {
                navMeshAgent.speed = navMeshAgent.speed * debuff.magnitude;
            }

            debuffList.Remove(debuff);
        }

        public virtual void addDebuff(SkillDetail debuff)
        {
            Debuff newDebuff = new Debuff();
            newDebuff = MapObject(debuff);

            ApplyAndRemoveDebuff(newDebuff.cancellationToken, newDebuff);
        }

        public Debuff MapObject(SkillDetail sd)
        {
            var result = new Debuff();
            result.magnitude = sd.magnitude;
            result.cooldown = sd.cooldown;
            result.duration = sd.duration;
            result.class_name = sd.class_name;
            result.context = sd.context;
            result.type = sd.type;
            return result;
        }

        private async Task ApplyAndRemoveDebuff(CancellationTokenSource cts, Debuff newDebuff)
        {
            try
            {
                applyDebuff(newDebuff);
                await Task.Delay(newDebuff.duration.ToMilliSeconds(), cts.Token);

            }
            catch
            {
            }
            finally {
                removeDebuff(newDebuff);
            }
        }
    }

}
