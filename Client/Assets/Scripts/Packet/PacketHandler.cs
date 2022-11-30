using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PacketHandler
{
    public static void S2C_ConnectedHandler(PacketSession session, IMessage packet)
    {
        Debug.Log("S2C_ConnectedHandler");
        C2S_Login loginPacket = new C2S_Login();
        loginPacket.UniqueId = SystemInfo.deviceUniqueIdentifier;
        Managers.Network.Send(loginPacket);
    }
    public static void S2C_LoginHandler(PacketSession session, IMessage packet)
    {
        S2C_Login loginPacket = (S2C_Login)packet;
        Debug.Log($"Success({loginPacket.BSuccess})");

        if(loginPacket.Players == null || loginPacket.Players.Count == 0)
        {
            C2S_CreatePlayer createPacket = new C2S_CreatePlayer();
            createPacket.Name = $"Player_{Random.Range(0, 10000).ToString("0000")}";
            Managers.Network.Send(createPacket);
        }
        else
        {
            LobbyPlayerInfo info = loginPacket.Players[0];
            C2S_EnterGame enterGamePacket = new C2S_EnterGame();
            enterGamePacket.Name =info.Name;
            Managers.Network.Send(enterGamePacket);
        }
    }
    public static void S2C_CreatePlayerHandler(PacketSession session, IMessage packet)
    {
        S2C_CreatePlayer createPlayerPacket = (S2C_CreatePlayer)packet;
        if(createPlayerPacket.Player == null)
        {
            C2S_CreatePlayer createPacket = new C2S_CreatePlayer();
            createPacket.Name = $"Player_{Random.Range(0, 10000).ToString("0000")}";
            Managers.Network.Send(createPacket);
        }
        else
        {
            C2S_EnterGame enterGamePacket = new C2S_EnterGame();
            enterGamePacket.Name = createPlayerPacket.Player.Name;
            Managers.Network.Send(enterGamePacket);
        }
    }
    public static void S2C_EnterGameHandler(PacketSession session, IMessage packet)
    {
        S2C_EnterGame enterGamePacket = packet as S2C_EnterGame;
        Managers.Object.Add(enterGamePacket.Player, true);
    }
    public static void S2C_LeaveGameHandler(PacketSession session, IMessage packet)
    {
        S2C_LeaveGame leaveGamePacket = packet as S2C_LeaveGame;
        Managers.Object.Clear();
    }
    public static void S2C_SpawnHandler(PacketSession session, IMessage packet)
    {
        S2C_Spawn spawnPacket = packet as S2C_Spawn;

        foreach (ObjectInfo player in spawnPacket.Objects)
        {
            Managers.Object.Add(player, myPlayer: false);
        }
    }
    public static void S2C_DespawnHandler(PacketSession session, IMessage packet)
    {
        S2C_Despawn despawnPacket = packet as S2C_Despawn;
        foreach (int playerId in despawnPacket.ObjectIds)
        {
            Managers.Object.Remove(playerId);
        }
    }
    public static void S2C_MoveHandler(PacketSession session, IMessage packet)
    {
        S2C_Move movePacket = packet as S2C_Move;

        GameObject go = Managers.Object.FindById(movePacket.ObjectId);
        if (go == null) return;

        if (Managers.Object.MyPlayer.Id == movePacket.ObjectId)
            return;

        BaseController bc = go.GetComponent<CreatureController>();
        if (bc == null) return;

        bc.PosInfo = movePacket.PosInfo;
    }
    public static void S2C_AttackHandler(PacketSession session, IMessage packet)
    {
        S2C_Attack attackPacket = packet as S2C_Attack;

        GameObject go = Managers.Object.FindById(attackPacket.ObjectId);
        if (go == null) return;

        CreatureController cc = go.GetComponent<CreatureController>();
        if (cc == null) return;

        cc.UseAttack();
    }
    public static void S2C_SkillHandler(PacketSession session, IMessage packet)
    {
        S2C_Skill skillPacket = packet as S2C_Skill;

        GameObject go = Managers.Object.FindById(skillPacket.ObjectId);
        if (go == null) return;

        PlayerController pc = go.GetComponent<PlayerController>();
        if (pc == null) return;

        pc.UseSkill(skillPacket.Info.SkillId);
    }
    public static void S2C_ChangeHpHandler(PacketSession session, IMessage packet)
    {
        S2C_ChangeHp changeHpPacket = packet as S2C_ChangeHp;

        GameObject go = Managers.Object.FindById(changeHpPacket.ObjectId);
        if (go == null) return;

        CreatureController cc = go.GetComponent<CreatureController>();
        if (cc == null) return;

        cc.Hp = changeHpPacket.Hp;
    }
    public static void S2C_DieHandler(PacketSession session, IMessage packet)
    {
        S2C_Die diePacket = packet as S2C_Die;

        GameObject go = Managers.Object.FindById(diePacket.ObjectId);
        if (go == null) return;

        CreatureController cc = go.GetComponent<CreatureController>();
        if (cc == null) return;

        cc.Hp = 0;
        cc.OnDead();
    }
}
