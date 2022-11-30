using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using Server;
using Server.DB;
using Server.Game;
using ServerCore;


internal class PacketHandler
{
    public static void C2S_LoginHandler(PacketSession session, IMessage packet)
    {
        C2S_Login Req_LoginPkt = (C2S_Login)packet;
        if (Req_LoginPkt == null) return;
        ClientSession clientSession = (ClientSession)session;
        if (clientSession == null) return;

        clientSession.HandleLogin(Req_LoginPkt);
    }
    public static void C2S_CreatePlayerHandler(PacketSession session, IMessage packet)
    {
        C2S_CreatePlayer Req_CreatePlayerPkt = (C2S_CreatePlayer)packet;
        if (Req_CreatePlayerPkt == null) return;
        ClientSession? clientSession = (ClientSession)session;
        if (clientSession == null) return;

        clientSession.HandleCreatePlayer(Req_CreatePlayerPkt);
    }
    public static void C2S_EnterGameHandler(PacketSession session, IMessage packet)
    {
        C2S_EnterGame Req_EnterGamePkt = (C2S_EnterGame)packet;
        if (Req_EnterGamePkt == null) return;
        ClientSession? clientSession = (ClientSession)session;
        if (clientSession == null) return;
        clientSession.HandleEnterGame(Req_EnterGamePkt);
    }
    public static void C2S_MoveHandler(PacketSession session, IMessage packet)
    {
        C2S_Move Req_MovePkt = (C2S_Move)packet;
        if (Req_MovePkt == null) return;
        ClientSession? clientSession = (ClientSession)session;
        if (clientSession == null) return;

        Player? player = clientSession.MyPlayer;
        if (player == null) return;

        GameRoom? room = player.Room;
        if (room == null) return;

        room.Push(room.HandleMove, player, Req_MovePkt);
    }
    public static void C2S_AttackHandler(PacketSession session, IMessage packet)
    {
        C2S_Attack Req_AttackPacket = (C2S_Attack)packet;
        if (Req_AttackPacket == null) return;
        ClientSession? clientSession = (ClientSession)session;
        if (clientSession == null) return;

        Player? player = clientSession.MyPlayer;
        if (player == null) return;

        GameRoom? room = player.Room;
        if (room == null) return;

        room.Push(room.HandleAttack, player, Req_AttackPacket);
    }
    public static void C2S_SkillHandler(PacketSession session, IMessage packet)
    {
        C2S_Skill Req_SkillPacket = (C2S_Skill)packet;
        if (Req_SkillPacket == null) return;
        ClientSession clientSession = (ClientSession)session;
        if (clientSession == null) return;

        Player? player = clientSession.MyPlayer;
        if (player == null) return;

        GameRoom? room = player.Room;
        if (room == null) return;

        room.Push(room.HandleSkill, player, Req_SkillPacket);
    }
}
