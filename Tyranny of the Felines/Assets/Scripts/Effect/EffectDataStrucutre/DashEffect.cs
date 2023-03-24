using System;
using UnityEngine;

namespace Matryoshka.Effect.EffectDataStructure
{
    [CreateAssetMenu(fileName = "New Dash Effect", menuName = "Effects/Dash Effect")]
    [Serializable]
    public class DashEffect : Effect
    {
        public float dashSpeed;
        public float dashDuration;

        public DashEffect() : base (EffectType.Dash)
        {

        }
    }
}

