using System;
using System.Collections;
using Matryoshka.Effect.EffectDataStructure;
using Matryoshka.Lobby;
using Matryoshka.ObjectPool;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

namespace Matryoshka.Effect.EffectController
{

	public enum ProjectileType
	{
		FeatherThrow,
		MagicMissile,
		SpitHairball
	}

    public class Projectile : NetworkBehaviour
    {
        private ProjectileEffect projectileEffect;
        private Vector2 direction;
        [SerializeField] 
        private GameObject successorPrefab;
        [SerializeField]
        private float successorChance = 1f;

        private int usedCount = 0;

        void OnEnable()
        {
            direction = new Vector2(0, 0);
        }

        public void Fire(ProjectileEffect projectileEffect, Vector2 direction)
        {
            this.projectileEffect = projectileEffect;
            this.direction = direction;
            if (projectileEffect.type == ProjectileType.MagicMissile)
            {
                if (LobbyManager.Singleton != null)
                {
                    LobbyManager.Singleton.PlaySoundClientRpc("PlaySalumonMagicMissile");
                }
            }
            if (projectileEffect.type == ProjectileType.FeatherThrow)
            {
                if (LobbyManager.Singleton != null)
                {
                    LobbyManager.Singleton.PlaySoundClientRpc("PlayWazoFeatherThrow");
                }
            }
            StartTimeout();
        }

        void FixedUpdate()
        {
            if (IsServer)
            {
                transform.position = new Vector3(
                    transform.position.x + (direction.x * projectileEffect.projectileSpeed),
                    transform.position.y + (direction.y * projectileEffect.projectileSpeed),
                    transform.position.z
                );
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

        private void Cleanup(bool spawnSuccessor)
        {
            StopTimeout();
            if (successorPrefab != null && spawnSuccessor && UnityEngine.Random.Range(0f, 1f) < successorChance)
            {
                NetworkObject networkObject = NetworkObjectPool.Singleton.GetNetworkObject(successorPrefab, transform.position, Quaternion.identity);
                networkObject.Spawn();

                Entity.Controller.IController e = networkObject.gameObject.GetComponent<Entity.Controller.IController>();
                if (e != null)
                {
                    e.Create();
                }

            }
            if (gameObject.GetComponent<NetworkObject>().IsSpawned)
            {
                gameObject.GetComponent<NetworkObject>().Despawn();
            }
        }
        private void SpecialCaseCleanup(){
            if (gameObject.GetComponent<NetworkObject>().IsSpawned)
            {
                gameObject.GetComponent<NetworkObject>().Despawn();
            }
        }

        private IEnumerator Timeout()
        {
            yield return new WaitForSeconds(projectileEffect.projectileDuration);
            Cleanup(true);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                if (other.gameObject.CompareTag("Wall"))
                {
                    Cleanup(true);
                }

                if (other.gameObject.CompareTag("Rock")){
                    Cleanup(true);
                }
                
                if (projectileEffect.target == EffectTargetType.Enemy)
                {
                    if(other.gameObject.CompareTag("Enemy")){
                        other.gameObject.GetComponent<Entity.Entity>().DoDamage(projectileEffect.projectileDamage);
                        Cleanup(true);
                    }
                }
                else if (projectileEffect.target == EffectTargetType.Player)
                {
                    if(other.gameObject.CompareTag("Player")){
                        other.gameObject.GetComponent<Entity.Entity>().DoDamage(projectileEffect.projectileDamage);
                        Cleanup(false);
                    }
                    else if (other.gameObject.CompareTag("Shield"))
                    {
                        other.gameObject.GetComponent<Shield>().Damage();
                        Cleanup(false);
                    }
                }
                else if (projectileEffect.target == EffectTargetType.EPP)
                {
                    if(other.gameObject.CompareTag("Player")){
                        other.gameObject.GetComponent<Entity.Entity>().AreaHealStart(projectileEffect.projectileHealing, projectileEffect.projectileDuration);
                    }
                    else if(other.gameObject.CompareTag("Enemy")){
                        other.gameObject.GetComponent<Entity.Entity>().DoDamage(projectileEffect.projectileDamage);
                        usedCount ++;
                        if(usedCount == projectileEffect.projectileUses){
                            Cleanup(false);
                        }
                        
                    }
                    else if(other.gameObject.CompareTag("Hairball")){
                        other.gameObject.GetComponent<Projectile>().SpecialCaseCleanup();
                        usedCount ++;
                        if(usedCount == projectileEffect.projectileUses){
                            Cleanup(false);
                        }
                    }
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other) {
            if (NetworkManager.Singleton.IsServer){
                if(projectileEffect.target == EffectTargetType.EPP)
                {
                    if(other.gameObject.CompareTag("Player")){
                        other.gameObject.GetComponent<Entity.Entity>().AreaHealEnd();
                    }
                }
            }
        }
    }
}