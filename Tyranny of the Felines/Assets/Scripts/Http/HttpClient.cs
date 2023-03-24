using System.Collections;
using System.Collections.Generic;
using System.Text;
using Matryoshka.Server;
using Matryoshka.UI;
using UnityEngine;
using UnityEngine.Networking;

namespace Matryoshka.Http
{
    public class HttpClient : MonoBehaviour
    {
        private static HttpClient _instance;
        public static HttpClient Singleton => _instance;

        private Server.Server server;

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
            DontDestroyOnLoad(this.gameObject);
        }

        public void GetServers()
        {
            StartCoroutine("GetServerListings");
        }

        public void HostServer(string ip, int port, string name)
        {
            server = new Server.Server("", name, 1, ip, port);
            StartCoroutine("UpsertServerListing");
        }
        public void HostServer(string joinCode, string name)
        {
            server = new Server.Server("", name, 1, joinCode);
            StartCoroutine("UpsertServerListing");
        }

        public void UpdateServerPlayerCount(int playerCount){
            server.players = playerCount;
            StartCoroutine("UpsertServerListing");
        }

        public void DeleteServer()
        {
            StartCoroutine("DeleteServerListing");
        }

        private IEnumerator UpsertServerListing() 
        {
            if (server.lobby_type == null || server.lobby_type == "")
            {
                yield return null;
            }
            string jsonString = JsonUtility.ToJson(server);
            UnityWebRequest webRequest = new UnityWebRequest(Constants.ServerAddress, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonString);
            webRequest.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("Error While Sending: " + webRequest.error);
            }
            else
            {
                CreateServerResponse response = JsonUtility.FromJson<CreateServerResponse>(webRequest.downloadHandler.text);
                if(response.success)
                {
                    server.id = response.id;
                }
                else {
                    Debug.Log($"Failed to get id from server. {webRequest.downloadHandler.text}");
                }
            }
            webRequest.Dispose();
        }

        private IEnumerator UpdateServerListing(Server.Server server)
        {
            string jsonString = JsonUtility.ToJson(server);
            UnityWebRequest webRequest = UnityWebRequest.Post(Constants.ServerAddress, jsonString);
            yield return webRequest.SendWebRequest();
            webRequest.Dispose();
        }

        private IEnumerator GetServerListings()
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(Constants.ServerAddress);
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("Error While Sending: " + webRequest.error);
            }
            else
            {
                Servers newServers = JsonUtility.FromJson<Servers>("{\"servers\":" + webRequest.downloadHandler.text + "}");
                LobbySelectorUI.Singleton.RenderServers(new List<Server.Server>(newServers.servers));
            }
            webRequest.Dispose();
        }

        private IEnumerator DeleteServerListing()
        {
            UnityWebRequest webRequest = UnityWebRequest.Delete($"{Constants.ServerAddress}{server.id}");
            yield return webRequest.SendWebRequest();
            webRequest.Dispose();
        }


    }
}

