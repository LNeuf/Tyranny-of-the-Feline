using System;
using UnityEngine;

namespace Matryoshka.Effect
{
    public enum EffectType
    {
        Projectile,
        Melee,
        MovementSpeed,
        Dash,
        AoE,
        Invulnerable,
        BulletHell,
        Animation,
        Catnip,
        Tower,
        Shield,
        DamageReduction
    }

    public enum EffectTargetType
    {
        Enemy,
        Player,
        PlayerHeal,
        EPP,
        BigPlayerHeal,
        None
    }
    
    [Serializable]
    public class Effect : ScriptableObject
    {
        [HideInInspector]
        internal EffectType effectType;

        public Effect(EffectType effectType)
        {
            this.effectType = effectType;
        }
    }
}