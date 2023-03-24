using System;
using System.Collections;
using Matryoshka.Effect.EffectDataStructure;
using Matryoshka.Lobby;
using Unity.Netcode;
using UnityEngine;

namespace Matryoshka.Effect.EffectController
{

	public enum AoEType
	{
		AoEHeal,
        AoEDamage,
        AoEMulti,
        AoETailSlam,
        AoETailSlamIndicator,
        AoEMultiIndicator
    }

    public class AoE : NetworkBehaviour
    {
        private AoEEffect areaEffect;
        
        public void Place(AoEEffect areaEffect, Vector2 position)
        {
            this.areaEffect = areaEffect;
            this.gameObject.GetComponent<Transform>().position = new Vector3(position.x,position.y,0);
            if (this.areaEffect.target == EffectTargetType.PlayerHeal)
            {
                if (LobbyManager.Singleton != null)
                {
                    LobbyManager.Singleton.PlaySoundClientRpc("PlaySalumonHeal");
                }
                
            }
            if (this.areaEffect.target == EffectTargetType.BigPlayerHeal)
            {
                if (LobbyManager.Singleton != null)
                {
                    LobbyManager.Singleton.PlaySoundClientRpc("PlaySalumonBigHeal");
                }
            }
            if (this.areaEffect.type == AoEType.AoEDamage && this.areaEffect.target == EffectTargetType.Enemy)
            {
                if (LobbyManager.Singleton != null)
                {
                    LobbyManager.Singleton.PlaySoundClientRpc("PlayWazoHailOfFeathers");
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
            yield return new WaitForSeconds(areaEffect.areaDuration);
            Cleanup();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                if (areaEffect.target == EffectTargetType.Enemy)
                {
                    if(other.gameObject.CompareTag("Enemy")){
                        other.gameObject.GetComponent<Entity.Entity>().OverTimeDamageStart(areaEffect.areaDamage, areaEffect.areaDuration);
                        
                    }   
                }
                else if (areaEffect.target == EffectTargetType.Player)
                {
                    if(other.gameObject.CompareTag("Player")){
                        other.gameObject.GetComponent<Entity.Entity>().DoDamage(areaEffect.areaDamage);
                        
                    }
                }
                else if(areaEffect.target == EffectTargetType.PlayerHeal)
                {
                    if(other.gameObject.CompareTag("Player")){
                        other.gameObject.GetComponent<Entity.Entity>().AreaHealStart(areaEffect.areaHealing, areaEffect.areaDuration);
                    }
                }
                else if(areaEffect.target == EffectTargetType.BigPlayerHeal)
                {
                    Debug.Log("Collide");
                    if (other.gameObject.CompareTag("Player") && !other.GetComponent<Entity.Entity>().IsDead())
                    {
                        other.gameObject.GetComponent<Entity.Entity>().Heal(areaEffect.areaHealing);
                    }
                    Cleanup();
                }
            }
        }
        private void OnTriggerExit2D(Collider2D other){
            if (NetworkManager.Singleton.IsServer){
                if(areaEffect.target == EffectTargetType.PlayerHeal)
                {
                    if(other.gameObject.CompareTag("Player")){
                        other.gameObject.GetComponent<Entity.Entity>().AreaHealEnd();
                    }
                }

                if(areaEffect.target == EffectTargetType.Enemy){
                    if(other.gameObject.CompareTag("Enemy")){
                        other.gameObject.GetComponent<Entity.Entity>().OverTimeDamageEnd();
                    }
                }
            }
        }
    }
}