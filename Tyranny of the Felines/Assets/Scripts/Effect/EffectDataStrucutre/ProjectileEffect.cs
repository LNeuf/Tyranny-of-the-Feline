using System;
using UnityEngine;
using Matryoshka.Effect.EffectController;

namespace Matryoshka.Effect.EffectDataStructure
{
    [CreateAssetMenu(fileName = "New Projectile Effect", menuName = "Effects/Projectile Effect")]
    [Serializable]
    public class ProjectileEffect : Effect
    {
        public int projectileDamage;
        public int projectileHealing;
        public int projectileUses;
        public float projectileSpeed;
        public float projectileDuration;
        public GameObject projectilePrefab;
		public ProjectileType type;
		public EffectTargetType target;

        public ProjectileEffect() : base (EffectType.Projectile)
        {

        }
    }
}
