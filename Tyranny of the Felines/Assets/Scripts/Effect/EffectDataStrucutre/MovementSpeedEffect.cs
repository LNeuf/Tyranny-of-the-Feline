using System;
using UnityEngine;

namespace Matryoshka.Effect.EffectDataStructure
{
    [CreateAssetMenu(fileName = "New Movement Speed Effect", menuName = "Effects/Movement Speed Effect")]
    [Serializable]
    public class MovementSpeedEffect : Effect
    {
        public float movementSpeed;

        public MovementSpeedEffect() : base (EffectType.MovementSpeed)
        {

        }
    }
}
