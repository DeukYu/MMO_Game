using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PacketHandler
{
    public static void S2C_EnterGameHandler(PacketSession session, IMessage packet)
    {
        S2C_EnterGame enterGamePacket = packet as S2C_EnterGame;
        Managers.Object.Add(enterGamePacket.Player, true);
        Debug.Log($"S2C_EnterGameHandler {enterGamePacket.Player}");
    }
    public static void S2C_LeaveGameHandler(PacketSession session, IMessage packet)
    {
        S2C_LeaveGame leaveGamePacket = packet as S2C_LeaveGame;
        Managers.Object.RemoveMyPlayer();
    }
    public static void S2C_SpawnHandler(PacketSession session, IMessage packet)
    {
        S2C_Spawn spawnPacket = packet as S2C_Spawn;

        foreach (PlayerInfo player in spawnPacket.Players)
        {
            Managers.Object.Add(player, myPlayer: false);
        }
    }
    public static void S2C_DespawnHandler(PacketSession session, IMessage packet)
    {
        S2C_Despawn despawnPacket = packet as S2C_Despawn;
        foreach (int playerId in despawnPacket.PlayerIds)
        {
            Managers.Object.Remove(playerId);
        }
    }
    public static void S2C_MoveHandler(PacketSession session, IMessage packet)
    {
        S2C_Move movePacket = packet as S2C_Move;
        ServerSession serverSession = session as ServerSession;

        Debug.Log("S2C_MoveHandler");
    }
}
