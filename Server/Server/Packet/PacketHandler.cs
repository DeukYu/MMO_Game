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
    public static void C2S_MoveHandler(PacketSession session, IMessage packet)
    {
        C2S_Move? req = packet as C2S_Move;
        if (req == null) return;
        ClientSession? clientSession = session as ClientSession;
        if(clientSession == null) return;

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

    public static void C2S_LoginHandler(PacketSession session, IMessage packet)
    {
        C2S_Login loginPacket = packet as C2S_Login;
        ClientSession clientSession = session as ClientSession;

        Console.WriteLine($"UniqueId({loginPacket.UniqueId})");

        // TODO : 보안 체크
        
        // TODO : Problem
        using(AppDbContext db = new AppDbContext())
        {
            AccountDb findAccount = db.Accounts
                .Where(a => a.AccountName == loginPacket.UniqueId).FirstOrDefault();

            if(findAccount != null)
            {
                S2C_Login res = new S2C_Login() { LoginOK = 1 };
                clientSession.Send(res);
            }
            else
            {
                AccountDb newAccount = new AccountDb() { AccountName = loginPacket.UniqueId };
                db.Accounts.Add(newAccount);
                db.SaveChanges();

                S2C_Login res = new S2C_Login() { LoginOK = 1 };
                clientSession.Send(res);
            }
        }
    }
}
