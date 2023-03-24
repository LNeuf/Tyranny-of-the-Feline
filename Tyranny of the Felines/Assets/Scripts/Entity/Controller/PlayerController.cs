using System;
using System.Collections;
using System.Collections.Generic;
using Matryoshka.Abilities;
using Matryoshka.Game;
using Matryoshka.UI;
using Unity.Netcode;
using UnityEngine;

namespace Matryoshka.Entity.Controller
{
    public class PlayerController : NetworkBehaviour, IController
    {
    
        private float centreX;
        private float centreY;
        private Camera playerCamera;
        private int playerIndex;
        private bool delayCameraChange;

        private const float LeftMouseButton = 1f;
        private const float RightMouseButton = 1f;
        private const float Q = 1f;
        private const float E = 1f;

        private List<AbilityType> leastRecentlyUsed;

        public void Create() { }

        private void Start()
        {
            playerIndex = 0;
            delayCameraChange = false;
            centreX = Screen.width / 2f;
            centreY = Screen.height / 2f;
            if (IsLocalPlayer)
            {
                playerCamera = GetComponent<Camera>();
                playerCamera.enabled = true;
            }

            leastRecentlyUsed = new List<AbilityType>
            {
                AbilityType.Special1,
                AbilityType.Secondary,
                AbilityType.Primary
            };
        }

        public Vector2 GetMovement()
        {
            if (GetComponent<Entity>().networkPlayerState.Value == PlayerState.Dead)
            {
                return new Vector2(0, 0);
            }
            float x = 0f;
            float y = 0f;
            
            if (!PauseMenuUI.Singleton.IsShowing() && !VictoryDefeatUI.Singleton.IsShowing())
            {
                x = Input.GetAxis("Horizontal");
                y = Input.GetAxis("Vertical");
            }

            return new Vector2(x, y);
        }

        public Vector2 GetMouseDirection()
        {
            Vector3 mousePosition = Input.mousePosition;
            float directionX = mousePosition.x - centreX;
            float directionY = mousePosition.y - centreY;
            Vector2 direction = new Vector2(directionX, directionY);
            return Utils.Utils.NormalizeToOne(direction);
        }

        public AbilityType GetAbility()
        {
            if (!PauseMenuUI.Singleton.IsShowing() && !VictoryDefeatUI.Singleton.IsShowing())
            {
                if (GetComponent<Entity>().networkPlayerState.Value == PlayerState.Dead)
                {
                    var playerObjects = GameObject.FindGameObjectsWithTag("Player");
                    if (Math.Abs(Input.GetAxis("Fire1") - LeftMouseButton) < 0.0001 && !delayCameraChange)
                    {
                        playerIndex -= 1;
                        if (playerIndex < 0)
                        {
                            playerIndex = playerObjects.Length - 1;
                        }

                        StartCoroutine(DelayCameraChange());
                    }
                    else if (Math.Abs(Input.GetAxis("Fire2") - RightMouseButton) < 0.0001 && !delayCameraChange)
                    {
                        playerIndex += 1;
                        if (playerIndex > playerObjects.Length - 1)
                        {
                            playerIndex = 0;
                        }
                        
                        StartCoroutine(DelayCameraChange());
                    }
                    playerCamera.enabled = false;
                    playerCamera = playerObjects[playerIndex].GetComponent<Camera>();
                    playerCamera.enabled = true;
                    return AbilityType.None;
                }

                bool foundLastUsed = false;
                AbilityType lastUsed = AbilityType.None;
                if (leastRecentlyUsed != null)
                {
                    foreach (var leastUsed in leastRecentlyUsed)
                    {
                        if (!foundLastUsed)
                        {
                            switch (leastUsed)
                            {
                                case AbilityType.Primary:
                                    if (Math.Abs(Input.GetAxis("Fire1") - LeftMouseButton) < 0.0001)
                                    {
                                        foundLastUsed = true;
                                        lastUsed = AbilityType.Primary;
                                    }

                                    break;
                                case AbilityType.Secondary:
                                    if (Math.Abs(Input.GetAxis("Fire2") - RightMouseButton) < 0.0001)
                                    {
                                        foundLastUsed = true;
                                        lastUsed = AbilityType.Secondary;
                                    }

                                    break;
                                case AbilityType.Special1:
                                    if (Math.Abs(Input.GetAxis("Ability1") - Q) < 0.0001)
                                    {
                                        foundLastUsed = true;
                                        lastUsed = AbilityType.Special1;
                                    }

                                    break;
                                default:
                                    Debug.Log("Unknown ability type in least recently used implementation: " +
                                              leastUsed.ToString());
                                    break;
                            }
                        }
                    }
                }

                if (lastUsed != AbilityType.None)
                {
                    leastRecentlyUsed.Remove(lastUsed);
                    leastRecentlyUsed.Add(lastUsed);
                }

                return lastUsed;
            }
            return AbilityType.None;
        }

        public Vector2 GetMousePosition(){
            Vector3 worldPosition = GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
            float positionX = worldPosition.x;
            float positionY = worldPosition.y;
            Vector2 position = new Vector2(positionX, positionY);
            return position;
        }

        //

        public PlayerState GetPlayerState()
        {
            if (GetMovement() == new Vector2(0, 0))
            {
                return PlayerState.Idle;
            }
            else
            {
                return PlayerState.Walk;
            }
        }

        private IEnumerator DelayCameraChange()
        {
            delayCameraChange = true;
            yield return new WaitForSeconds(0.5f);
            delayCameraChange = false;
        }
    }
}
