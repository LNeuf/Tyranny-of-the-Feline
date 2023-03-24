using Matryoshka.Effect.EffectController;
using Matryoshka.Effect.EffectDataStrucutre;
using Matryoshka.Entity.Controller;
using Matryoshka.Game;
using Matryoshka.ObjectPool;
using Unity.Netcode;
using UnityEngine;

namespace Matryoshka.Effect.EffectSpawner
{
    public class MittensEffectSpawner : MonoBehaviour, EffectSpawner
    {

        public GameObject catnipPrefab;
        public GameObject towerPrefab;

        public void SpawnCatnip(Effect effect, EntityInfo entityInfo)
        {
            NetworkObject catnipObject = NetworkObjectPool.Singleton.GetNetworkObject(
                catnipPrefab,
                GetPosition(entityInfo.position), Quaternion.identity);
            catnipObject.Spawn(true);
            Catnip catnip = catnipObject.gameObject.GetComponent<Catnip>();
            catnip.Create();
        }

        public void SpawnTower(Effect effect, EntityInfo entityInfo)
        {
            NetworkObject towerObject = NetworkObjectPool.Singleton.GetNetworkObject(
                towerPrefab,
                GetPosition(entityInfo.position), Quaternion.identity);
            CatTower tower = towerObject.gameObject.GetComponent<CatTower>();
            GameObject mittens = GameManager.Singleton.mittensObject;
            mittens.GetComponent<Animator>().SetBool("Sleeping", true);
            mittens.GetComponent<EnemyController>().sleeping = true;
            mittens.GetComponent<Entity.Entity>().SetInvulnerability(true);
            tower.Create();
            towerObject.Spawn(true);
        }

        public Vector3 GetPosition(Vector2 position)
        {
            return new Vector3(position.x, position.y) + GetOffset();
        }

        public void Spawn(Effect effect, EntityInfo entityInfo)
        {
            Debug.Log($"EffectSpawner: {effect.name} {effect.effectType}");
            switch (effect.effectType)
            {
                case EffectType.Tower:
                    SpawnTower(effect, entityInfo);
                    break;
                case EffectType.Catnip:
                    SpawnCatnip(effect, entityInfo);
                    break;
                default:
                    Debug.LogError($"Unable to spawn effect {effect.effectType}");
                    break;
            }
        }

        public Vector3 GetOffset()
        {
            // TODO when it is not 3:31 am. refactor when not tired, move position to mittens entity.
            var angle = GameManager.Singleton.mittensObject.GetComponent<BulletSpawner>().positionOffset;
            switch (angle)
            {
                case 90:
                    return new Vector3(10, 0); // left
                case 180:
                    return new Vector3(0, -10); // top
                case 270:
                    return new Vector3(-10, 0); // right
                case 0:
                default:
                    return new Vector3(0, 10); // bottom
            }
        }
    }
}