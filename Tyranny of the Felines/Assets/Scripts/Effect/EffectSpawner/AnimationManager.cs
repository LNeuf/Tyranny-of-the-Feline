using Matryoshka.Abilities;
using Matryoshka.Effect.EffectDataStrucutre;
using Matryoshka.Entity;
using Matryoshka.Game;
using UnityEngine;

namespace Matryoshka.Effect.EffectSpawner
{
    public class AnimationManager : MonoBehaviour
    {
        public void ActivateAnimation(AnimationEffect effect, EntityInfo entityInfo)
        {
            GameObject gameObjectToAnimate;
            if (entityInfo.entityType == EntityType.Mittens)
            {
                gameObjectToAnimate = GameManager.Singleton.mittensObject;
            }
            else
            {
                gameObjectToAnimate = GameManager.GetEntityWithType(entityInfo.entityType);
            }
            Entity.Entity entity = gameObjectToAnimate.GetComponent<Entity.Entity>();
            entity.SetAnimation(effect.animationToDo);
        }
    }
}