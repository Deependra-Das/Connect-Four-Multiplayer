using System;
using Unity.Collections;
using Unity.Netcode;

[System.Serializable]
public struct PlayerSessionData : INetworkSerializable, IEquatable<PlayerSessionData>
{
    public ulong clientId;
    public FixedString64Bytes playerId;
    public FixedString64Bytes username;
    public int winsCount;

    public PlayerSessionData(ulong clientId, string playerId, string username, int winsCount)
    {
        this.clientId = clientId;
        this.playerId = playerId;
        this.username = new FixedString64Bytes(username);
        this.winsCount = winsCount;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref playerId);
        serializer.SerializeValue(ref username);
        serializer.SerializeValue(ref winsCount);
    }

    public bool Equals(PlayerSessionData other)
    {
        return clientId == other.clientId &&
               playerId.Equals(other.playerId);
    }
}