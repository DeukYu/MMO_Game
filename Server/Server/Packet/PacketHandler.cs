﻿using System;
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
        C2S_Login Req_LoginPacket = (C2S_Login)packet;
        if (Req_LoginPacket == null) return;
        ClientSession clientSession = (ClientSession)session;
        if(clientSession == null) return;

        Console.WriteLine($"UniqueId({Req_LoginPacket.UniqueId})");

        // TODO : 보안 체크

        // TODO : Problem
        using (AppDbContext db = new AppDbContext())
        {
            AccountDb findAccount = db.Accounts
                .Where(a => a.AccountName == Req_LoginPacket.UniqueId).FirstOrDefault();

            if (findAccount != null)
            {
                S2C_Login res = new S2C_Login() { BSuccess = true };
                clientSession.Send(res);
            }
            else
            {
                AccountDb newAccount = new AccountDb() { AccountName = Req_LoginPacket.UniqueId };
                db.Accounts.Add(newAccount);
                db.SaveChanges();

                S2C_Login res = new S2C_Login() { BSuccess = true };
                clientSession.Send(res);
            }
        }
    }
    public static void C2S_MoveHandler(PacketSession session, IMessage packet)
    {
        C2S_Move Req_MovePacket = (C2S_Move)packet;
        if (Req_MovePacket == null) return;
        ClientSession? clientSession = session as ClientSession;
        if(clientSession == null) return;

        Player? player = clientSession.MyPlayer;
        if (player == null) return;

        GameRoom? room = player.Room;
        if (room == null) return;

        room.Push(room.HandleMove, player, Req_MovePacket);
    }
    public static void C2S_AttackHandler(PacketSession session, IMessage packet)
    {
        C2S_Attack Req_AttackPacket = (C2S_Attack)packet;
        if(Req_AttackPacket == null) return;
        ClientSession clientSession = session as ClientSession;
        if(clientSession == null) return;

        Player? player = clientSession.MyPlayer;
        if (player == null)
            return;

        GameRoom? room = player.Room;
        if (room == null)
            return;

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
        if (room != null)
            room.Push(room.HandleSkill, player, Req_SkillPacket);
    }
}
