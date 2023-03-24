using Matryoshka.Abilities;
using Matryoshka.Effect.EffectDataStructure;
using Matryoshka.ObjectPool;
using Unity.Netcode;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Matryoshka.Effect.EffectController;

namespace Matryoshka.Effect.EffectSpawner
{
	[Serializable]
	public struct MeleePrefab
	{
		public MeleeType meleeType;
		public GameObject meleePrefab;
	}

    public class MeleeSpawner : MonoBehaviour, EffectSpawner
    {
        public List<MeleePrefab> meleePrefabs;

        public void Spawn(Effect effect, EntityInfo entityInfo)
        {
	        MeleeEffect meleeEffect = (MeleeEffect)effect;
	        NetworkObject meleeObject = NetworkObjectPool.Singleton.GetNetworkObject(
				GetMeleePrefab(meleeEffect.type), 
                entityInfo.position + GetDirectionOffset(Utils.Utils.ConvertToEight(entityInfo.mouseDirection), meleeEffect.type), 
				Utils.Utils.CalculateRotationFromVector(Utils.Utils.ConvertToEight(entityInfo.mouseDirection), 
				GetRotationOffset(meleeEffect.type)));
            meleeObject.Spawn(true);
            Melee melee = meleeObject.gameObject.GetComponent<Melee>();
            melee.Slash(meleeEffect);
        }

		private Vector2 GetDirectionOffset(Vector2 direction, MeleeType type)
		{
			if (type == MeleeType.Slash)
			{
				float offset = 0.5f;
				if (Math.Abs(MathF.Abs(direction.x) - MathF.Abs(direction.y)) < 0.0001)
				{
					offset /= MathF.Sqrt(2f);
				}

				return direction * offset;
			} 
			else if (type == MeleeType.OverheadSlash)
			{
				float offset = 1.0f;
				if (Math.Abs(MathF.Abs(direction.x) - MathF.Abs(direction.y)) < 0.0001)
				{
					offset /= MathF.Sqrt(2f);
				}
				
				return direction * offset;
			}
			else if (type == MeleeType.ClawSwipe || type == MeleeType.ClawSwipeIndicator)
			{
				return Utils.Utils.GetClawSwipeOffset(direction);
				
			}
			Debug.Log("!!!Could not find directional offset for Melee Type!!!");
			return new Vector2(0,0);
		}

		private GameObject GetMeleePrefab(MeleeType type)
		{
			foreach (var prefab in meleePrefabs)
            {
                if (prefab.meleeType == type)
                {
                    return prefab.meleePrefab;
                }
            }

            Debug.Log("!!!Could not find Melee Type!!!");
            return null;
		}

		private float GetRotationOffset(MeleeType type)
		{
			if (type == MeleeType.Slash)
			{
				return 158f;
			} 
			else if (type == MeleeType.OverheadSlash)
			{
				return 90f;
			}
			else if (type == MeleeType.ClawSwipe || type == MeleeType.ClawSwipeIndicator)
			{
				return 90f;
			}
			Debug.Log("!!!Could not find rotation offset for Melee Type!!!");
			return 0f;
		}
    }
}
