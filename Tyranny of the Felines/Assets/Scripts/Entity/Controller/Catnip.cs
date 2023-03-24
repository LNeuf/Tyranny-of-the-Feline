using Matryoshka.Abilities;
using Matryoshka.Entity;
using Matryoshka.Entity.Controller;
using Matryoshka.Game;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Matryoshka.Effect.EffectController
{
    public class Catnip : NetworkBehaviour, IController
    {
        private const float CatnipDuration = 10f;

        public void Create()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                var e = GetComponent<Entity.Entity>();
                e.HealToFull();
                GameManager.GetMittens().GetComponent<Entity.Entity>().SetNipOrTowerOnScreen(true);
                Debug.Log("Catnip spawned");
                StartTimeout();
            }
        }
    
        private void StartTimeout()
        {
            StartCoroutine("Timeout");
        }

        private void StopTimeout()
        {
            StopCoroutine("Timeout");
            StopCoroutine("Rage");
        }

        private void Cleanup()
        {
            Debug.Log("Catnip destroyed");
            GameManager.GetMittens().GetComponent<Entity.Entity>().SetNipOrTowerOnScreen(false);
            StopTimeout();
            gameObject.GetComponent<NetworkObject>().Despawn();
        }

        public void DestroyedByPlayer()
        {
            Debug.Log("Catnip destroyed safely");
            Cleanup();
        }

        public void Rage()
        {
            var spawner = GameManager.Singleton.mittensObject.GetComponent<BulletSpawner>();
            Debug.Log("Rage");
            spawner.BeginAngerPhase();
        }

        private IEnumerator Timeout()
        {
            yield return new WaitForSeconds(CatnipDuration);
            Cleanup();
            if (NetworkManager.Singleton.IsServer && GetComponent<Entity.Entity>().Health > 0)
            {
                Debug.Log("Catnip destroyed rage");
                Rage(); // Rage lasts 9 seconds
            }
        }

        public Vector2 GetMovement()
        {
            return new Vector2(0, 0);
        }

        public Vector2 GetMouseDirection()
        {
            return new Vector2(0, 0);
        }

        public AbilityType GetAbility()
        {
            return AbilityType.None;
        }

        public Vector2 GetMousePosition()
        {
            return new Vector2(0, 0);
        }

        public PlayerState GetPlayerState()
        {
            return PlayerState.Idle;
        }
    }
}