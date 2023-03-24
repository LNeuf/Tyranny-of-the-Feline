using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matryoshka.Effect.EffectDataStructure
{
    [CreateAssetMenu(fileName = "New Invulnerable Effect", menuName = "Effects/Invulnerable Effect")]
    [Serializable]
    public class InvulnerableEffect : Effect
    {
        public bool isInvulnerable;

        public InvulnerableEffect() : base(EffectType.Invulnerable)
        {
            
        }
    }
}
