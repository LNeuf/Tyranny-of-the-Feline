using Matryoshka.Abilities;
using Matryoshka.Effect.EffectDataStructure;
using Matryoshka.ObjectPool;
using Unity.Netcode;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Matryoshka.Effect.EffectController;
using Matryoshka.Game;

namespace Matryoshka.Effect.EffectSpawner
{
    [Serializable]
    public struct ProjectilePrefab
    {
        public ProjectileType projectileType;
        public GameObject projectilePrefab;
    }
    public class ProjectileSpawner : MonoBehaviour, EffectSpawner
    {
        public List<ProjectilePrefab> projectilePrefabs;

        public void Spawn(Effect effect, EntityInfo entityInfo)
        {
            ProjectileEffect projectileEffect = (ProjectileEffect)effect;
            Vector2 direction = GetDirection(entityInfo, projectileEffect);
            
            NetworkObject projectileObject = NetworkObjectPool.Singleton.GetNetworkObject(
                GetProjectilePrefab(projectileEffect.type),
                entityInfo.position, GetRotation(direction, projectileEffect));
            if(ProjectileType.MagicMissile == projectileEffect.type){
                projectileObject.transform.Translate(direction*2);
            }
            
            projectileObject.Spawn(true);
            Projectile projectile = projectileObject.gameObject.GetComponent<Projectile>();
            projectile.Fire(projectileEffect, direction);
        }

        private Vector2 GetDirection(EntityInfo entityInfo, ProjectileEffect effect)
        {
            if (effect.type == ProjectileType.SpitHairball)
            {
                GameObject mittens = GameManager.GetMittens();
                Vector3 entityDistance = Utils.Utils.VectorDifference(
                    GameManager.GetRandomPlayer().transform.position,
                    mittens.transform.position);
                Vector3 direction = Utils.Utils.NormalizeToOne(new Vector2(entityDistance.x, entityDistance.y));
                return Utils.Utils.NudgeVector2(new Vector2(direction.x, direction.y));
            }
            return entityInfo.mouseDirection;
        }
        
        private Quaternion GetRotation(Vector2 direction, ProjectileEffect effect)
        {
            if (effect.type == ProjectileType.SpitHairball)
            {
                return Utils.Utils.CalculateRotationFromVector(direction, 0);

            }
            else if (effect.type == ProjectileType.FeatherThrow)
            {
                return Utils.Utils.CalculateRotationFromVector(direction, -45f);
            }
            return Utils.Utils.CalculateRotationFromVector(direction);
        }

        private GameObject GetProjectilePrefab(ProjectileType type)
        {
            foreach (var prefab in projectilePrefabs)
            {
                if (prefab.projectileType == type)
                {
                    return prefab.projectilePrefab;
                }
            }

            Debug.Log("!!!Could not find Projectile Type!!!");
            return null;
        }
    }
}
