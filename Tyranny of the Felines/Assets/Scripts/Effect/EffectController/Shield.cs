using System.Collections;
using Matryoshka.Effect.EffectDataStructure;
using Matryoshka.Game;
using Unity.Netcode;
using UnityEngine;

namespace Matryoshka.Effect.EffectController
{
    public class Shield : MonoBehaviour
    {
        private int hitsToBreak;
        private ShieldEffect shieldEffect;
        
        public void Create(ShieldEffect effect, EntityInfo entityInfo)
        {
            shieldEffect = effect;
            hitsToBreak = shieldEffect.hitsToBreak;
            GameObject caster = GameManager.GetEntityWithType(entityInfo.entityType);
            caster.GetComponent<Entity.Entity>().shield = gameObject;
            StartTimeout();
        }
        
        public void Damage() {
            hitsToBreak--;
            if (hitsToBreak <= 0) {
                Cleanup();
            }
        }
        
        private void StartTimeout()
        {
            StartCoroutine("Timeout");
        }

        private void StopTimeout()
        {
            StopCoroutine("Timeout");
        }
        
        private IEnumerator Timeout()
        {
            yield return new WaitForSeconds(shieldEffect.shieldDuration);
            Cleanup();
        }

        // private void OnTriggerEnter2D(Collider2D other)
        // {
        //     if (NetworkManager.Singleton.IsServer)
        //     {
        //     }
        // }

        private void Cleanup()
        {
            StopTimeout();
            if (gameObject.GetComponent<NetworkObject>().IsSpawned)
            {
                gameObject.GetComponent<NetworkObject>().Despawn();
            }
        }
    }
    
    
}
