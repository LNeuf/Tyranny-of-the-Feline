using System;
using UnityEngine;
using Matryoshka.Effect.EffectController;

namespace Matryoshka.Effect.EffectDataStructure
{
    [CreateAssetMenu(fileName = "New Area Effect", menuName = "Effects/Area Effect")]
    [Serializable]
    public class AoEEffect : Effect
    {
        public int areaDamage;
        public int areaHealing;
        public float areaDuration;
        public GameObject areaPrefab;
		public AoEType type;
		public EffectTargetType target;

        public AoEEffect() : base (EffectType.AoE)
        {

        }
    }
}
