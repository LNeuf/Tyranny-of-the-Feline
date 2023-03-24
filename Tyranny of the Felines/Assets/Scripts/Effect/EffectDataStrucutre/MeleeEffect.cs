using System;
using UnityEngine;
using Matryoshka.Effect.EffectController;

namespace Matryoshka.Effect.EffectDataStructure
{
    [CreateAssetMenu(fileName = "New Melee Effect", menuName = "Effects/Melee Effect")]
    [Serializable]
    public class MeleeEffect : Effect
    {
        public int meleeDamage;
        public float meleeDuration;
        public GameObject meleePrefab;
		public MeleeType type;
		public EffectTargetType target;

        public MeleeEffect() : base (EffectType.Melee)
        {

        }
    }
}
