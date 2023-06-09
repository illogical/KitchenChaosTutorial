using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    public ulong ClientID;
    public int ColorId;
    public FixedString64Bytes PlayerName;
    public FixedString64Bytes PlayerId;

    public bool Equals(PlayerData other) => ClientID == other.ClientID
        && ColorId == other.ColorId
        && PlayerName == other.PlayerName
        && PlayerId == other.PlayerId;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientID);
        serializer.SerializeValue(ref ColorId);
        serializer.SerializeValue(ref PlayerName);
        serializer.SerializeValue(ref PlayerId);
    }
}
