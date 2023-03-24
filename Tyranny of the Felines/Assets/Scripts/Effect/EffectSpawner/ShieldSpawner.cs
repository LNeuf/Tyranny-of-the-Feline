using System.Collections;
using System.Collections.Generic;
using Matryoshka.Effect.EffectController;
using Matryoshka.Effect.EffectDataStructure;
using Matryoshka.Effect.EffectSpawner;
using Matryoshka.ObjectPool;
using Unity.Netcode;
using UnityEngine;

namespace Matryoshka.Effect.EffectSpawner
{
    public class ShieldSpawner : MonoBehaviour, EffectSpawner
    {
        public GameObject shieldPrefab;

        public void Spawn(Effect effect, EntityInfo entityInfo)
        {
            ShieldEffect shieldEffect = (ShieldEffect)effect;
            Vector2 direction = GetDirection(entityInfo, shieldEffect);
            Vector3 position = new Vector3(entityInfo.position.x +  direction.x,
                entityInfo.position.y + direction.y);
            NetworkObject projectileObject = NetworkObjectPool.Singleton.GetNetworkObject(
                shieldPrefab,
                position, GetRotation(direction, shieldEffect));
            projectileObject.Spawn(true);
            Shield shield = projectileObject.gameObject.GetComponent<Shield>();
            shield.Create(shieldEffect, entityInfo);
        }
        
        private Vector2 GetDirection(EntityInfo entityInfo, ShieldEffect effect)
        {
            return Utils.Utils.ConvertToEight(entityInfo.mouseDirection);
        }
        
        private Quaternion GetRotation(Vector2 direction, ShieldEffect effect)
        {
            return Utils.Utils.CalculateRotationFromVector(direction);
        }
    } 
}

