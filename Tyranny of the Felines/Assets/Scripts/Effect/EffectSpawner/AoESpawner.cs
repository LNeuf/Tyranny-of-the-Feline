using Matryoshka.Abilities;
using Matryoshka.Effect.EffectDataStructure;
using Matryoshka.ObjectPool;
using Unity.Netcode;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Matryoshka.Effect.EffectController;
using Matryoshka.Entity;
using Matryoshka.Game;
using Matryoshka.Lobby;

namespace Matryoshka.Effect.EffectSpawner
{
    
    [Serializable]
    public struct AoEPrefab
    {
        public AoEType aoeType;
        public GameObject aoePrefab;
    }


    public class AoESpawner : MonoBehaviour, EffectSpawner
    {
        public const float ArenaLeft = -21f;
        public const float ArenaRight = 21f;
        public const float ArenaTop = 15f;
        public const float ArenaBottom = -17f;
        public const int ImpaleCount = 10;
        public static readonly float ArenaWidth = (Math.Abs(ArenaLeft) + Math.Abs(ArenaRight));
        public static readonly float ArenaHeight = (Math.Abs(ArenaTop) + Math.Abs(ArenaBottom));
        public static readonly float ArenaOrginX = ArenaWidth / 2 + ArenaLeft;
        public static readonly float ArenaOrginY = ArenaHeight / 2 + ArenaBottom;

        private Vector2 TailSlamPosition;
        private Vector2[] ImpalePositions = new Vector2[ImpaleCount];

        public static bool InOval(float x, float y, float rx, float ry, float oval_x, float oval_y)
        {
            return ((x - oval_x) * (x - oval_x) / (rx * rx)) + ((y - oval_y) * (y - oval_y) / (ry * ry)) <= 1;
        }

        public static bool InArena(float x, float y) => InOval(x, y, ArenaWidth / 2, ArenaHeight / 2, ArenaOrginX, ArenaOrginY);
        public static bool InArenaPadded(float x, float y) => InOval(x, y, ArenaWidth / 2-2, ArenaHeight / 2-2, ArenaOrginX, ArenaOrginY);
        public List<AoEPrefab> aoePrefabs;
        
        public void Spawn(Effect effect, EntityInfo entityInfo)
        {
            AoEEffect aoeEffect = (AoEEffect)effect;

            if (entityInfo.entityType == EntityType.Mittens)
            {
                switch (aoeEffect.type)
                {
                    case AoEType.AoEMulti:
                    case AoEType.AoEMultiIndicator:
                        SpawnImpales(aoeEffect, entityInfo);
                        break;
                    case AoEType.AoETailSlam:
                    case AoEType.AoETailSlamIndicator:
                        SpawnTailSlam(aoeEffect, entityInfo);
                        break;
                }
            } else
            {
                Vector2 position = GetCursorPosition(entityInfo);
                NetworkObject aoeObject =
                    NetworkObjectPool.Singleton.GetNetworkObject(GetAoEPrefab(aoeEffect.type));
                aoeObject.Spawn(true);
                AoE aoe = aoeObject.gameObject.GetComponent<AoE>();
                aoe.Place(aoeEffect, position);
            }
        }

        private void SpawnImpales(AoEEffect aoeEffect, EntityInfo entityInfo)
        {
            for (int i = 0; i < ImpaleCount; i++)
            {
                Vector2 position = ImpalePositions[i];
                if (aoeEffect.type == AoEType.AoEMultiIndicator)
                {
                    float x = UnityEngine.Random.Range(ArenaLeft, ArenaRight);
                    float y = UnityEngine.Random.Range(ArenaBottom, ArenaTop);

                    if (!InArenaPadded(x, y)) { i--; continue; }
                    position = new Vector2(x, y);
                    ImpalePositions[i] = position;
                }

                NetworkObject aoeObject =
                    NetworkObjectPool.Singleton.GetNetworkObject(GetAoEPrefab(aoeEffect.type), position, Quaternion.identity);
                aoeObject.Spawn(true);
                AoE aoe = aoeObject.gameObject.GetComponent<AoE>();
                aoe.Place(aoeEffect, position);
                if (LobbyManager.Singleton != null)
                {
                    LobbyManager.Singleton.PlaySoundClientRpc("PlayMittensSpikes");
                }
            }

        }
        private void SpawnTailSlam(AoEEffect aoeEffect, EntityInfo entityInfo)
        {
            if (aoeEffect.type == AoEType.AoETailSlamIndicator)
            {
                float x = UnityEngine.Random.Range(-20.0f, 20.0f);
                Vector2 position = new Vector2(x, 5.0f);
                int randomNumber = 0;
                if (GameManager.Singleton.playerObjects.Count == 2)
                {
                    randomNumber = UnityEngine.Random.Range(0, 2);
                    if (GameManager.Singleton.playerObjects[randomNumber].GetComponent<Entity.Entity>().IsDead())
                    {
                        if (randomNumber == 0) { randomNumber = 1; }
                        else { randomNumber = 0; }
                    }
                }
                else if (GameManager.Singleton.playerObjects.Count == 3)
                {
                    randomNumber = UnityEngine.Random.Range(0, 3);
                    if (randomNumber == 0)
                    {
                        if (GameManager.Singleton.playerObjects[randomNumber].GetComponent<Entity.Entity>().IsDead())
                        {
                            randomNumber = UnityEngine.Random.Range(1, 3);
                            if (GameManager.Singleton.playerObjects[randomNumber].GetComponent<Entity.Entity>().IsDead())
                            {
                                if (randomNumber == 2) { randomNumber = 1; }
                                else { randomNumber = 2; }
                            }
                        }
                    }
                    else if (randomNumber == 1)
                    {
                        if (GameManager.Singleton.playerObjects[randomNumber].GetComponent<Entity.Entity>().IsDead())
                        {
                            randomNumber = UnityEngine.Random.Range(1, 3);
                            if (randomNumber == 1) { randomNumber = 0; }
                            if (GameManager.Singleton.playerObjects[randomNumber].GetComponent<Entity.Entity>().IsDead())
                            {
                                if (randomNumber == 0) { randomNumber = 2; }
                                else { randomNumber = 0; }
                            }
                        }
                    }
                    else
                    {
                        if (GameManager.Singleton.playerObjects[randomNumber].GetComponent<Entity.Entity>().IsDead())
                        {
                            randomNumber = UnityEngine.Random.Range(0, 2);
                            if (GameManager.Singleton.playerObjects[randomNumber].GetComponent<Entity.Entity>().IsDead())
                            {
                                if (randomNumber == 0) { randomNumber = 1; }
                                else { randomNumber = 0; }
                            }
                        }
                    }
                }

                float enemyPositionX = GameManager.Singleton.playerObjects[randomNumber].GetComponent<Transform>().position.x;
                float enemyPositionY = GameManager.Singleton.playerObjects[randomNumber].GetComponent<Transform>().position.y;

                if (entityInfo.mouseDirection == Vector2.left)
                {
                    position.x = 8;
                    position.y = enemyPositionY;
                }
                else if (entityInfo.mouseDirection == Vector2.up)
                {
                    position.x = enemyPositionX;
                }
                else if (entityInfo.mouseDirection == Vector2.right)
                {
                    position.x = -8;
                    position.y = enemyPositionY;
                }
                else
                {
                    position.x = enemyPositionX;
                }
                TailSlamPosition = position;
                //StartCoroutine(GracePeriod(aoeEffect));
            }

            if (aoeEffect.type == AoEType.AoETailSlam || aoeEffect.type == AoEType.AoETailSlamIndicator)
            {
                NetworkObject aoeObject =
                        NetworkObjectPool.Singleton.GetNetworkObject(GetAoEPrefab(aoeEffect.type), TailSlamPosition, GetRotation(entityInfo.mouseDirection, aoeEffect));
                aoeObject.Spawn(true);
                AoE aoe = aoeObject.gameObject.GetComponent<AoE>();
                aoe.Place(aoeEffect, TailSlamPosition);
                if (LobbyManager.Singleton != null)
                {
                    LobbyManager.Singleton.PlaySoundClientRpc("PlayMittensTeleport");
                }
            }
        }

        private GameObject GetAoEPrefab(AoEType type)
        {
            foreach (var prefab in aoePrefabs)
            {
                if (prefab.aoeType == type)
                {
                    return prefab.aoePrefab;
                }
            }

            Debug.Log("!!!Could not find AoE Type!!!");
            return null;
        }

        private Vector2 GetCursorPosition(EntityInfo entityInfo)
        {
            return entityInfo.mousePosition;
        }

        private Vector2 GetDirection(EntityInfo entityInfo, AoEEffect effect)
        {
            return entityInfo.mouseDirection;
        }

        private Quaternion GetRotation(Vector2 direction, AoEEffect effect)
        {
            return Utils.Utils.CalculateRotationFromVector(direction);
        }
    }
}



