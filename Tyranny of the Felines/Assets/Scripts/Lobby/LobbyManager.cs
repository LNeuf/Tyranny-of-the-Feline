using System;
using System.Collections.Generic;
using Matryoshka.UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Video;

namespace Matryoshka.Lobby
{
    public class LobbyManager : NetworkBehaviour
    {
        private static LobbyManager _instance;
        public static LobbyManager Singleton => _instance;

        private bool spawned = false;
        public NetworkList<LobbyPlayer> Players;

        public GameObject introVideoPrefab;
    
        public void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        void Start()
        {
            if (Players != null)
            {
                Players.OnListChanged += (_) => { LobbyUI.Singleton.RenderPlayers(GetPlayerList()); };
                LobbyUI.Singleton.RenderPlayers(GetPlayerList());
            }
        }

        void OnEnable()
        {
            SpawnIfUnspawned();
        }

        public void SpawnIfUnspawned()
        {
            if (NetworkManager.Singleton.IsServer && !spawned)
            {
                spawned = true;
                Players = new NetworkList<LobbyPlayer>();
                this.gameObject.GetComponent<NetworkObject>().Spawn();
                Players.OnListChanged += (_) => { LobbyUI.Singleton.RenderPlayers(GetPlayerList()); };
                LobbyUI.Singleton.RenderPlayers(GetPlayerList());
            }
        }

        private int FindIndexOfClient(ulong clientId)
        {
            for(int i = 0; i < Players.Count; i++) 
            {
                LobbyPlayer player = Players[i];
                if (player.GetId() == clientId) 
                {
                    return i;
                } 
            }
            return -1;
        }

        private bool HasPlayerClass(PlayerClass playerClass)
        {
            for(int i = 0; i < Players.Count; i++) 
            {
                LobbyPlayer player = Players[i];
                if (player.GetPlayerClass() == playerClass) 
                {
                    return true;
                } 
            }
            return false;
        }

        [ServerRpc(RequireOwnership=false)]
        public void RequestSetCharacterServerRpc(ulong clientId, PlayerClass playerClass, ServerRpcParams rpcParams = default)
        {
            SetCharacter(clientId, playerClass, rpcParams);
        }

        public void SetCharacter(ulong clientId, PlayerClass playerClass, ServerRpcParams rpcParams = default)
        {
            int index = FindIndexOfClient(clientId);
            if (index  == -1)
            {
                Debug.Log($"Something went wrong, the clientId {clientId} doesn't exist.");
            }
            else
            {
                if (!HasPlayerClass(playerClass) || playerClass == PlayerClass.None) {
                    LobbyPlayer oldPlayer = Players[index];
                    LobbyPlayer newPlayer = new LobbyPlayer(oldPlayer.GetId(), oldPlayer.GetName(), playerClass); 
                    Players[index] = newPlayer;
                }
            }
        }

        [ClientRpc]
        public void StartVideoClientRpc()
        {
            GameUI.Singleton.HideScreen();
            GameObject videoPlayerObject = Instantiate(introVideoPrefab);
            VideoPlayer videoPlayer = videoPlayerObject.GetComponent<VideoPlayer>();
            videoPlayer.targetCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            videoPlayer.Play();
            PlaySoundClientRpc("PlayMenuMusic");
        }

        public List<LobbyPlayer> GetPlayerList() 
        {
            List<LobbyPlayer> list = new List<LobbyPlayer>();
            for(int i = 0; i < Players.Count; i++) {
                list.Add(Players[i]);
            }
            return list;
        }

        public bool Validate()
        {
            for(int i = 0; i < Players.Count; i++) 
            {
                LobbyPlayer player = Players[i];
                if (player.GetPlayerClass() == PlayerClass.None) 
                {
                    return false;
                } 
            }
            return true;
        }

        [ClientRpc]
        public void PlaySoundClientRpc(string method)
        {
            SoundManager.Singleton.SendMessage(method);
        }

    }
}
