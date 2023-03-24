using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matryoshka.Effect.EffectDataStructure
{
    [CreateAssetMenu(fileName = "New Damage Reduction Effect", menuName = "Effects/Damage Reduction Effect")]
    [Serializable]
    public class DamageReductionEffect : Effect
    {
        public float damageReductionAmount;

        public DamageReductionEffect() : base(EffectType.DamageReduction)
        {
            
        }
    }
}
