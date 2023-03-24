using Matryoshka.Effect;

namespace Matryoshka.Effect.EffectSpawner
{
    public interface EffectSpawner 
    {
        void Spawn(Effect effect, EntityInfo entityInfo);
    }
}
