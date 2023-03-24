using System.Collections.Generic;
using UnityEngine;
using Matryoshka.Effect;
using Matryoshka.Effect.EffectDataStructure;
using Matryoshka.Effect.EffectDataStrucutre;
using Matryoshka.Game;

namespace Matryoshka.Effect.EffectSpawner
{
    public class EffectManager : MonoBehaviour
    {
        private static EffectManager _instance;
        public static EffectManager Singleton => _instance;
    
        public void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        public void Apply(List<Effect> effects, EntityInfo entityInfo)
        {
            Entity.Entity entity;
            GameObject entityGameObject;
            foreach(Effect effect in effects)
            {
                switch(effect.effectType)
                {
                    case EffectType.Projectile:
                        ProjectileSpawner projectileSpawner = this.gameObject.GetComponent<ProjectileSpawner>();
                        projectileSpawner.Spawn(effect, entityInfo);
                        break;
					case EffectType.Melee:
						MeleeSpawner meleeSpawner = this.gameObject.GetComponent<MeleeSpawner>();
						meleeSpawner.Spawn(effect, entityInfo);
                        break;
					case EffectType.MovementSpeed:
						MovementSpeedEffect movementSpeedEffect = (MovementSpeedEffect) effect;
                        entityGameObject = GameManager.GetEntityWithType(entityInfo.entityType);
                        entity = entityGameObject.GetComponent<Entity.Entity>();
						entity.SetMovementSpeed(movementSpeedEffect.movementSpeed);
                        break;
                    case EffectType.Dash:
                        DashEffect dashEffect = (DashEffect) effect;
                        DashManager dashManager = this.gameObject.GetComponent<DashManager>();
                        entityGameObject = GameManager.GetEntityWithType(entityInfo.entityType);
                        entity = entityGameObject.GetComponent<Entity.Entity>();
                        DashInfo dashInfo = new DashInfo(entity, entityInfo.moveDirection, dashEffect);
                        dashManager.StartDash(dashInfo);
                        break;
                    case EffectType.AoE:
                        AoESpawner aoeSpawner = this.gameObject.GetComponent<AoESpawner>();
                        aoeSpawner.Spawn(effect, entityInfo);
                        break;
                    case EffectType.Invulnerable:
                        InvulnerableEffect invulnerableEffect = (InvulnerableEffect) effect;
                        entityGameObject = GameManager.GetEntityWithType(entityInfo.entityType);
                        entity = entityGameObject.GetComponent<Entity.Entity>();
                        entity.SetInvulnerability(invulnerableEffect.isInvulnerable);
                        break;
                    case EffectType.DamageReduction:
                        DamageReductionEffect damageReductionEffect = (DamageReductionEffect) effect;
                        entityGameObject = GameManager.GetEntityWithType(entityInfo.entityType);
                        entity = entityGameObject.GetComponent<Entity.Entity>();
                        entity.SetDamageReductionPercentage(damageReductionEffect.damageReductionAmount);
                        break;
                    case EffectType.Catnip:
                    case EffectType.Tower:
                        MittensEffectSpawner mittensEffectSpawner = gameObject.GetComponent<MittensEffectSpawner>();
                        mittensEffectSpawner.Spawn(effect, entityInfo);
                        break;
                    case EffectType.Animation:
                        AnimationManager animationManager = gameObject.GetComponent<AnimationManager>();
                        animationManager.ActivateAnimation((AnimationEffect) effect, entityInfo);
                        break;
                    case EffectType.Shield:
                        ShieldSpawner shieldSpawner = gameObject.GetComponent<ShieldSpawner>();
                        shieldSpawner.Spawn((ShieldEffect) effect, entityInfo);
                        break;
                    default:
                        Debug.Log("Default Effect");
                        break;
                }
            }
        }
    }
}
