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
    public bool isReady;

    public PlayerSessionData(ulong clientId, string playerId, string username, int winsCount, bool isReady)
    {
        this.clientId = clientId;
        this.playerId = playerId;
        this.username = new FixedString64Bytes(username);
        this.winsCount = winsCount;
        this.isReady = isReady;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref playerId);
        serializer.SerializeValue(ref username);
        serializer.SerializeValue(ref winsCount);
        serializer.SerializeValue(ref isReady);
    }

    public bool Equals(PlayerSessionData other)
    {
        return clientId == other.clientId &&
               playerId.Equals(other.playerId) &&
                username.Equals(other.username) &&
                 winsCount.Equals(other.winsCount) &&
                 isReady.Equals(other.isReady);
    }
}