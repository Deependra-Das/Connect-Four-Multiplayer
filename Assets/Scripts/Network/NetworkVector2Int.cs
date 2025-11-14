using Unity.Netcode;
using UnityEngine;
using System;

namespace ConnectFourMultiplayer.Network
{
    [Serializable]
    public struct NetworkVector2Int :
        INetworkSerializable, IEquatable<NetworkVector2Int>
    {
        public int x;
        public int y;

        public NetworkVector2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public NetworkVector2Int(Vector2Int v)
        {
            x = v.x;
            y = v.y;
        }

        public Vector2Int ToVector2Int()
        {
            return new Vector2Int(x, y);
        }

        public bool Equals(NetworkVector2Int other)
        {
            return x == other.x && y == other.y;
        }

        public override bool Equals(object obj)
        {
            return obj is NetworkVector2Int other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer)
            where T : IReaderWriter
        {
            serializer.SerializeValue(ref x);
            serializer.SerializeValue(ref y);
        }
    }
}
