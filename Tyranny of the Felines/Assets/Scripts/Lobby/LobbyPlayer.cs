using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Matryoshka.Lobby {
    public enum PlayerClass
    {
        Mouse,
        Bird,
        Fish,
        None
    }

    public struct LobbyPlayer : INetworkSerializable, IEquatable<LobbyPlayer>
    {
        private ulong id;
        private FixedString32Bytes name;
        private PlayerClass playerClass;


        public LobbyPlayer(ulong Id, FixedString32Bytes Name, PlayerClass PlayerClass) 
        {
            id = Id;
            name = Name;
            playerClass = PlayerClass;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T: IReaderWriter
        {
            serializer.SerializeValue(ref id);
            serializer.SerializeValue(ref name);
            serializer.SerializeValue(ref playerClass);
        }

        public bool Equals(LobbyPlayer other)
        {
            return playerClass == other.playerClass && name.Equals(other.name);
        }

        public override bool Equals(object other)
        {
            return other is LobbyPlayer && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(playerClass, name);
        }

        public ulong GetId()
        {
            return id;
        }

        public FixedString32Bytes GetName()
        {
            return name;
        }

        public PlayerClass GetPlayerClass()
        {
            return playerClass;
        }

    }

}
