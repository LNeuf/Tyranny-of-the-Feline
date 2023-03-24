using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matryoshka.Effect.EffectDataStructure
{
    [CreateAssetMenu(fileName = "New Shield Effect", menuName = "Effects/Shield Effect")]
    [Serializable]
    public class ShieldEffect : Effect
    {
        public int hitsToBreak;
        public float shieldDuration;
        
        public ShieldEffect() : base(EffectType.Shield)
        {
        }
    }
}
