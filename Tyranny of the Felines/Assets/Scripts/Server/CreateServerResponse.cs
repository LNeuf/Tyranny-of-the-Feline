using System;

namespace Matryoshka.Server
{
    [Serializable]
    public struct CreateServerResponse
    {
        public string id;
        public bool success;
    }
}
