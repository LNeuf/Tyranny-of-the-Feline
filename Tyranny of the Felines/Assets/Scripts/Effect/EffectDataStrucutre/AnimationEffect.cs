using System;
using Matryoshka.Abilities;
using UnityEngine;

namespace Matryoshka.Effect.EffectDataStrucutre
{

    [CreateAssetMenu(fileName = "New Animation Effect", menuName = "Effects/Animation Effect")]
    [Serializable]
    public class AnimationEffect : Effect
    {
        public AbilityType animationToDo;

        public AnimationEffect() : base(EffectType.Animation)
        {
        }
    }
}