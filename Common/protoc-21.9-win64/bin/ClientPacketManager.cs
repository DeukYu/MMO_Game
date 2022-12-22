using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
{
	#region Singleton
	static PacketManager _instance = new PacketManager();
	public static PacketManager Instance { get { return _instance; } }
	#endregion

	PacketManager()
	{
		Register();
	}

	Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
	Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();
		
	public Action<PacketSession, IMessage, ushort> CustomHandler { get; set; }

	public void Register()
	{
        _onRecv.Add((ushort)MsgId.S2CConnected, MakePacket<S2C_Connected>);
        _handler.Add((ushort)MsgId.S2CConnected, PacketHandler.S2C_ConnectedHandler);
        _onRecv.Add((ushort)MsgId.S2CLogin, MakePacket<S2C_Login>);
        _handler.Add((ushort)MsgId.S2CLogin, PacketHandler.S2C_LoginHandler);
        _onRecv.Add((ushort)MsgId.S2CCreatePlayer, MakePacket<S2C_CreatePlayer>);
        _handler.Add((ushort)MsgId.S2CCreatePlayer, PacketHandler.S2C_CreatePlayerHandler);
        _onRecv.Add((ushort)MsgId.S2CEnterGame, MakePacket<S2C_EnterGame>);
        _handler.Add((ushort)MsgId.S2CEnterGame, PacketHandler.S2C_EnterGameHandler);
        _onRecv.Add((ushort)MsgId.S2CLeaveGame, MakePacket<S2C_LeaveGame>);
        _handler.Add((ushort)MsgId.S2CLeaveGame, PacketHandler.S2C_LeaveGameHandler);
        _onRecv.Add((ushort)MsgId.S2CSpawn, MakePacket<S2C_Spawn>);
        _handler.Add((ushort)MsgId.S2CSpawn, PacketHandler.S2C_SpawnHandler);
        _onRecv.Add((ushort)MsgId.S2CDespawn, MakePacket<S2C_Despawn>);
        _handler.Add((ushort)MsgId.S2CDespawn, PacketHandler.S2C_DespawnHandler);
        _onRecv.Add((ushort)MsgId.S2CMove, MakePacket<S2C_Move>);
        _handler.Add((ushort)MsgId.S2CMove, PacketHandler.S2C_MoveHandler);
        _onRecv.Add((ushort)MsgId.S2CAttack, MakePacket<S2C_Attack>);
        _handler.Add((ushort)MsgId.S2CAttack, PacketHandler.S2C_AttackHandler);
        _onRecv.Add((ushort)MsgId.S2CSkill, MakePacket<S2C_Skill>);
        _handler.Add((ushort)MsgId.S2CSkill, PacketHandler.S2C_SkillHandler);
        _onRecv.Add((ushort)MsgId.S2CChangeHp, MakePacket<S2C_ChangeHp>);
        _handler.Add((ushort)MsgId.S2CChangeHp, PacketHandler.S2C_ChangeHpHandler);
        _onRecv.Add((ushort)MsgId.S2CDie, MakePacket<S2C_Die>);
        _handler.Add((ushort)MsgId.S2CDie, PacketHandler.S2C_DieHandler);
        _onRecv.Add((ushort)MsgId.S2CItemlist, MakePacket<S2C_Itemlist>);
        _handler.Add((ushort)MsgId.S2CItemlist, PacketHandler.S2C_ItemlistHandler);
        _onRecv.Add((ushort)MsgId.S2CAddItem, MakePacket<S2C_AddItem>);
        _handler.Add((ushort)MsgId.S2CAddItem, PacketHandler.S2C_AddItemHandler);
	}

	public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
	{
		ushort count = 0;

		ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
		count += 2;
		ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
		count += 2;

		Action<PacketSession, ArraySegment<byte>, ushort> action = null;
		if (_onRecv.TryGetValue(id, out action))
			action.Invoke(session, buffer, id);
	}

	void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
	{
		T pkt = new T();
		pkt.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count - 4);
		
		if (CustomHandler != null)
		{
			CustomHandler.Invoke(session, pkt, id);
		}
		else
		{
			Action<PacketSession, IMessage> action = null;
			if (_handler.TryGetValue(id, out action))
				action.Invoke(session, pkt);
		}
	}

	public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
	{
		Action<PacketSession, IMessage> action = null;
		if (_handler.TryGetValue(id, out action))
			return action;
		return null;
	}
}