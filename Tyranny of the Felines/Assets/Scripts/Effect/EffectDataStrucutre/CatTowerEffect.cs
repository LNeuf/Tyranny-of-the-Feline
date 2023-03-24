using UnityEngine;
using System;

namespace Matryoshka.Effect.EffectDataStrucutre
{
    [CreateAssetMenu(fileName = "New CatTower Effect", menuName = "Effects/CatTower Effect")]
    [Serializable]
    public class CatTowerEffect : Effect
    {
        
        public CatTowerEffect() : base(EffectType.Tower)
        {
        }
    }
}