using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Complete
{

    [Serializable]
    public class SkillDetail
    {
        public string requiresTarget;
        public float duration;
        public float magnitude;
        public string type;
        public float cooldown;
        public float projectileDamage;
        public string skill_name;
        public string description;
        public string skill_sprite_name;
        public string class_name;

        public int modifier_count = 0;
        public int id;

        public string icon_name => skill_sprite_name + "Icon";
        public bool enabled = true;
        public string context;
    }

    public abstract class AbstractSkillDetail : SkillDetail
    {
        public abstract void Execute(CombatManager context);
    }

    public class Debuff : SkillDetail
    {
        public CancellationTokenSource cancellationToken = new CancellationTokenSource();
    }

    [Serializable]
    public abstract class SkillModifier : AbstractSkillDetail
    {
        public override void Execute(CombatManager context)
        {
            ModifierObject am = new ModifierObject();
            am.info = this;


            context.addSkillModifier(am);
        }
    }

    [Serializable]
    public abstract class AttackModifier : AbstractSkillDetail
    {
        public override void Execute(CombatManager context)
        {
            ModifierObject am = new ModifierObject();
            am.info = this;

            context.addAttackModifier(am);
        }

        public abstract void ApplyAttackModifier(ref AttackProjectile projectile, ModifierObject modifier);
    }

    [Serializable]
    public class BlinkSkill : AbstractSkillDetail
    {
        private Animator blinkAnim;
        private GameObject blinkAnimation;
        public override void Execute(CombatManager context)
        {
            blinkAnimation = Resources.Load<GameObject>("Skills/BlinkAnimation");
            blinkAnim = blinkAnimation.GetComponent<Animator>();
            context.player.transform.position = context.FindMousePointRelativeToPlayerWithPlayerY(context.player);

            blinkAnim.Play("Arive");

            blinkAnimation.transform.position = context.player.transform.position;
        }
    }

    [Serializable]
    public class AbstractPlaceTerrain : AbstractSkillDetail
    {
        public override void Execute(CombatManager context)
        {
            var result = PlaceTerrain(context.terrainCancelationToken, context);
        }

        public async Task PlaceTerrain(CancellationTokenSource cancellationTokenSource, CombatManager context) {
            GameObject terrainGO = new GameObject();
            TerrainManager terrain;
            try
            {
                terrainGO = GameObject.Instantiate(Resources.Load<GameObject>("TerrainTemplate"));
                terrain = terrainGO.GetComponent<TerrainManager>();
                terrainGO.SetActive(true);
                terrain.DisableColliders();
                context.SetCurrentTerrain(terrainGO);
                context.isPlacingTerrain = true;
                while (context.isPlacingTerrain) {
                    await Task.Delay(200, cancellationTokenSource.Token);
                }
                terrain.Spawn();
            }
            catch
            {
                terrainGO.SetActive(false);
            }
            finally {

            }
        }
    }

    public class DodgeRoll : AbstractSkillDetail {
        public override void Execute(CombatManager context)
        {
            context.playerManager.DodgeRoll();
        }
    }

    public class SplitShot : AttackModifier
    {
        public override void Execute(CombatManager context)
        {
            base.Execute(context);
        }

        public override void ApplyAttackModifier(ref AttackProjectile projectile, ModifierObject modifier)
        {
            modifier.info.context = "Birth";
            projectile.AddModifier(modifier);
        }
    }

    public class SizeSkill : AttackModifier
    {
        public override void Execute(CombatManager context)
        {
            base.Execute(context);
        }

        public override void ApplyAttackModifier(ref AttackProjectile projectile, ModifierObject modifier)
        {
            projectile.rigidBody.gameObject.transform.localScale = projectile.rigidBody.gameObject.transform.localScale * modifier.info.magnitude;
        }
    }

    public class DurationSkill : SkillModifier
    {
        public override void Execute(CombatManager context)
        {
            base.Execute(context);
        }
    }


    public class SpawnGlacier : AbstractPlaceTerrain
    {
        public override void Execute(CombatManager context)
        {
            base.Execute(context);
        }
    }

    public class SlowSkill : AttackModifier
    {
        public override void Execute(CombatManager context)
        {
            base.Execute(context);
        }

        public override void ApplyAttackModifier(ref AttackProjectile projectile, ModifierObject modifier)
        {
            modifier.info.context = "Enemy";
            projectile.AddModifier(modifier);
        }
    }

    public class BounceSkill : AttackModifier
    {
        public override void Execute(CombatManager context)
        {
            base.Execute(context);
        }

        public override void ApplyAttackModifier(ref AttackProjectile projectile, ModifierObject modifier) {
            projectile.AddBounces(Mathf.RoundToInt(modifier.info.magnitude));
        }
    }

    public class DamageSkill : AttackModifier
    {
        public override void Execute(CombatManager context)
        {
            base.Execute(context);
        }

        public override void ApplyAttackModifier(ref AttackProjectile projectile, ModifierObject modifier)
        {
            projectile.damage = projectile.damage * modifier.info.magnitude;
        }
    }


}
