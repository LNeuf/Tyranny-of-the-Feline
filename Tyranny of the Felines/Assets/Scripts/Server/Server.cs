using System;

namespace Matryoshka.Server
{
    [Serializable]
    public struct Server
    {
        public string id;
        public string name;
        public int players;
        public string ip;
        public ushort port;
        public string join_code;
        public string lobby_type; // TODO enum

        public Server(string Id, string Name, int Players, string Ip, int Port){
            id = Id;
            name = Name;
            players = Players;
            ip = Ip;
            port = (ushort)Port;
            join_code = "";
            lobby_type = "Direct";
        }

        public Server(string Id, string Name, int Players, string JoinCode)
        {
            id = Id;
            name = Name;
            players = Players;
            join_code = JoinCode;
            ip = "";
            port = 0;
            lobby_type = "Relay";
        }
    }
}
