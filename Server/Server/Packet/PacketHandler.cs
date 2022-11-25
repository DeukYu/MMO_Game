using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using Server.Game;
using ServerCore;


internal class PacketHandler
{
    public static void C2S_MoveHandler(PacketSession session, IMessage packet)
    {
        C2S_Move? req = packet as C2S_Move;
        if (req == null) return;
        ClientSession? clientSession = session as ClientSession;
        if(clientSession == null) return;

        Console.WriteLine($"C2S_Move ({req.PosInfo.PosX}, {req.PosInfo.PosY})");

        Player? player = clientSession.MyPlayer;
        if (player == null)
            return;

        GameRoom? room = player.Room;
        if (room == null)
            return;

        room.Push(room.HandleMove, player, req);
        // TODO : 검증
    }
    public static void C2S_AttackHandler(PacketSession session, IMessage packet)
    {
        C2S_Attack? req = packet as C2S_Attack;
        if(req == null) return;
        ClientSession clientSession = session as ClientSession;
        if(clientSession == null) return;

        Player? player = clientSession.MyPlayer;
        if (player == null)
            return;

        GameRoom? room = player.Room;
        if (room == null)
            return;

        room.Push(room.HandleAttack, player, req);
    }
    public static void C2S_SkillHandler(PacketSession session, IMessage packet)
    {
        C2S_Skill? req = packet as C2S_Skill;
        if (req == null) return;
        ClientSession clientSession = session as ClientSession;
        if (clientSession == null) return;

        Player? player = clientSession.MyPlayer;
        if (player == null)
            return;

        GameRoom? room = player.Room;
        if (room != null)
            room.Push(room.HandleSkill, player, req);
    }
}
