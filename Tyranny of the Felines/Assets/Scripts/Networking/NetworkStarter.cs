using Unity.Netcode;
using UnityEngine;

namespace Matryoshka.Networking
{
    public class NetworkStarter : MonoBehaviour
    {
        [SerializeField]
        private GameObject networkManagerPrefab;
        void Start()
        {
            if (NetworkManager.Singleton == null)
            {
                Instantiate(networkManagerPrefab, Vector3.zero, Quaternion.identity);
            }
        }
    }
}
