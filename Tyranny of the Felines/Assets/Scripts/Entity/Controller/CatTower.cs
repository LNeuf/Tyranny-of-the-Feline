using System;
using Matryoshka.Abilities;
using Matryoshka.Game;
using System.Collections;
using System.Collections.Generic;
using Matryoshka.Lobby;
using Unity.Netcode;
using UnityEngine;

namespace Matryoshka.Entity.Controller
{
    public class CatTower : MonoBehaviour, IController
    {
        [SerializeField]
        public int healPercentage = 0;
        [SerializeField]
        public float healSpeed = 0.5f; // in seconds

        // Start is called before the first frame update
        public void Create()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                var e = this.GetComponent<Entity>();
                e.HealToFull();
                GetComponent<Animator>().Play("TowerRise");
                GameManager.GetMittens().GetComponent<Entity>().SetNipOrTowerOnScreen(true);
                Debug.Log($"tower spawned {e.GetHealthValue()} {e.GetMaxHealthValue()}");
                InvokeRepeating("MittensTowerHealLoop", 0f, 0.5f);
            }
        }

        public void Destroy()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                GameManager.GetMittens().GetComponent<Entity>().SetNipOrTowerOnScreen(false);
                CancelInvoke("MittensTowerHealLoop");
                GameObject mittens = GameManager.Singleton.mittensObject;
                mittens.GetComponent<Animator>().SetBool("Sleeping", false);
                mittens.GetComponent<EnemyController>().sleeping = false;
                mittens.GetComponent<Entity>().SetInvulnerability(false);
                Debug.Log("tower destroyed");
                gameObject.GetComponent<NetworkObject>().Despawn();
                LobbyManager.Singleton.PlaySoundClientRpc("PlayTowerDamaged");
            }
        }

        public void OnDestroy()
        {
            CancelInvoke("MittensTowerHealLoop");
        }

        private void MittensTowerHealLoop()
        {
            //Debug.Log("tower heal");
            if (GameManager.Singleton.mittensObject != null)
            {
                var mittens = GameManager.Singleton.mittensObject.GetComponent<Entity>();
                mittens.Heal(healPercentage);
            }
        }

        public void UpdateDamageFrame()
        {
            var entity = GetComponent<Entity>();
            // Animation takes 1 second;
            GetComponent<Animator>().Play("TowerBreak", 0, 1 - (entity.GetHealthValue() / entity.GetMaxHealthValue()));

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
