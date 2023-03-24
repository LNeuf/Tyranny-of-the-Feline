using UnityEngine;
using System;

namespace Matryoshka.Effect.EffectDataStrucutre
{
    [CreateAssetMenu(fileName = "New Catnip Effect", menuName = "Effects/Catnip Effect")]
    [Serializable]
    public class CatnipEffect : Effect
    {
        
        public CatnipEffect() : base(EffectType.Catnip)
        {
        }
    }
}