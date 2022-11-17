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
        _onRecv.Add((ushort)MsgId.S2CEnterGame, MakePacket<S2C_EnterGame>);
        _handler.Add((ushort)MsgId.S2CEnterGame, PacketHandler.S2C_EnterGameHandler);
        _onRecv.Add((ushort)MsgId.S2CLeaveGame, MakePacket<S2C_LeaveGame>);
        _handler.Add((ushort)MsgId.S2CLeaveGame, PacketHandler.S2C_LeaveGameHandler);
        _onRecv.Add((ushort)MsgId.S2CSpawn		, MakePacket<S2C_Spawn		>);
        _handler.Add((ushort)MsgId.S2CSpawn		, PacketHandler.S2C_Spawn		Handler);
        _onRecv.Add((ushort)MsgId.S2CDespawn	, MakePacket<S2C_Despawn	>);
        _handler.Add((ushort)MsgId.S2CDespawn	, PacketHandler.S2C_Despawn	Handler);
        _onRecv.Add((ushort)MsgId.S2CMove		, MakePacket<S2C_Move		>);
        _handler.Add((ushort)MsgId.S2CMove		, PacketHandler.S2C_Move		Handler);
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