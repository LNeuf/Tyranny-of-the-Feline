using System;
using Matryoshka.Abilities;
using Matryoshka.Game;
using Matryoshka.ObjectPool;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Matryoshka.Entity.Controller
{
    public class FurballController : MonoBehaviour, IController
    {
        [SerializeField]
        public int damage;
        [SerializeField]
        public GameObject target;

        private float last_target = 0;


        public void Start() 
        {
            Create();
        }

        public void Create()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                Entity mittens = GameManager.GetMittens().GetComponent<Entity>();
                var e = GetComponent<Entity>();
                e.maxHealth.Value = Math.Max(15, 15 * (mittens.mittensPhase + 1));
                e.HealToFull();
                FindTarget();

                if (e.healthBar == null)
                {
                    NetworkObject healthBar = NetworkObjectPool.Singleton.GetNetworkObject(GameManager.Singleton.healthBar,
                        this.transform.position, this.transform.rotation);
                    healthBar.Spawn(true);
                    e.SetHealthBar(healthBar.gameObject);
                }
            }
        }


        public void FindTarget()
        {
            float dist = Mathf.Infinity;
            foreach (var p in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (p.GetComponent<Entity>().networkPlayerState.Value == PlayerState.Dead)
                {
                    continue;
                }
                
                var d = Utils.Utils.VectorDistance(p.transform.position, this.transform.position);

                if (d < dist)
                {
                    dist = d;
                    SetTarget(p);
                }
            }
        }

        public Vector2 GetMovement()
        {

            if (target == null || Time.time - last_target >= 1)
            {
                FindTarget();
            }

            if (target == null)
            {
                return new Vector2(0, 0);
            }

            return Utils.Utils.NormalizeToOne(Utils.Utils.VectorDifference(target.transform.position, this.transform.position));
        }

        public Vector2 GetMouseDirection()
        {
            return new Vector2(0, 0);
        }

        public AbilityType GetAbility()
        {
            if (GetPlayerState() == PlayerState.Dead)
            {
                return AbilityType.None;
            }
            int randomNumber = Random.Range(0, 2);
            if (randomNumber == 0)
            {
                return AbilityType.Primary;
            }
            else if (randomNumber == 1)
            {
                return AbilityType.Secondary;
            }

            return AbilityType.None;
        }

        //Should not get called but needed to implement
        public Vector2 GetMousePosition()
        {
            Vector2 position = new Vector2(0, 0);

            return position;
        }

        //

        public PlayerState GetPlayerState()
        {
            return GetComponent<Entity>().networkPlayerState.Value;
        }

        public void SetTarget(GameObject target)
        {
            this.target = target;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                if (other.gameObject.CompareTag("Player"))
                {
                    other.gameObject.GetComponent<Entity>().DoDamage(damage);
                }
            }
        }
    }
}
