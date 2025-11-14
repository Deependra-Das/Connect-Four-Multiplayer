using System;
using Unity.Netcode;
using UnityEngine;

namespace ConnectFourMultiplayer.Network
{
    [Serializable]
    public struct NetworkVector3Int : INetworkSerializable, IEquatable<NetworkVector3Int>
    {
        public int x;
        public int y;
        public int z;

        public NetworkVector3Int(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public NetworkVector3Int(Vector3Int v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        public Vector3Int ToVector3Int()
        {
            return new Vector3Int(x, y, z);
        }

        public bool Equals(NetworkVector3Int other)
        {
            return x == other.x && y == other.y && z == other.z;
        }

        public override bool Equals(object obj)
        {
            return obj is NetworkVector3Int other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y, z);
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref x);
            serializer.SerializeValue(ref y);
            serializer.SerializeValue(ref z);
        }
    }
}