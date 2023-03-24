using System.Collections;
using Matryoshka.Effect.EffectDataStructure;
using Matryoshka.Lobby;
using Unity.Netcode;
using UnityEngine;

namespace Matryoshka.Effect.EffectController
{
    public enum MeleeType
    {
        Slash,
        OverheadSlash,
        ClawSwipe,
        ClawSwipeIndicator
    }
    public class Melee : NetworkBehaviour
    {
        private MeleeEffect meleeEffect;

        public void Slash(MeleeEffect meleeEffect)
        {
            this.meleeEffect = meleeEffect;
            if (this.meleeEffect.type == MeleeType.Slash)
            {
                if (LobbyManager.Singleton != null)
                {
                    LobbyManager.Singleton.PlaySoundClientRpc("PlayBigSlash");
                }
            }

            if (this.meleeEffect.type == MeleeType.OverheadSlash)
            {
                if (LobbyManager.Singleton != null)
                {
                    LobbyManager.Singleton.PlaySoundClientRpc("PlayBigOverheadSlash");
                }
            }

            StartTimeout();
        }
        private void StartTimeout()
        {
            StartCoroutine("Timeout");
        }

        private void StopTimeout()
        {
            StopCoroutine("Timeout");
        }

        private void Cleanup()
        {
            StopTimeout();
            if (gameObject.GetComponent<NetworkObject>().IsSpawned)
            {
                gameObject.GetComponent<NetworkObject>().Despawn();
            }
        }

        private IEnumerator Timeout()
        {
            yield return new WaitForSeconds(meleeEffect.meleeDuration);
            Cleanup();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                if ((meleeEffect.target == EffectTargetType.Enemy && other.gameObject.CompareTag("Enemy")) ||
                    (meleeEffect.target == EffectTargetType.Player && other.gameObject.CompareTag("Player"))) {
                    other.gameObject.GetComponent<Entity.Entity>().DoDamage(meleeEffect.meleeDamage);
                    Cleanup();
                }
            }
        }
    }
}
