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
        _onRecv.Add((ushort)MsgId.C2SLogin, MakePacket<C2S_Login>);
        _handler.Add((ushort)MsgId.C2SLogin, PacketHandler.C2S_LoginHandler);
        _onRecv.Add((ushort)MsgId.C2SCreatePlayer, MakePacket<C2S_CreatePlayer>);
        _handler.Add((ushort)MsgId.C2SCreatePlayer, PacketHandler.C2S_CreatePlayerHandler);
        _onRecv.Add((ushort)MsgId.C2SEnterGame, MakePacket<C2S_EnterGame>);
        _handler.Add((ushort)MsgId.C2SEnterGame, PacketHandler.C2S_EnterGameHandler);
        _onRecv.Add((ushort)MsgId.C2SMove, MakePacket<C2S_Move>);
        _handler.Add((ushort)MsgId.C2SMove, PacketHandler.C2S_MoveHandler);
        _onRecv.Add((ushort)MsgId.C2SAttack, MakePacket<C2S_Attack>);
        _handler.Add((ushort)MsgId.C2SAttack, PacketHandler.C2S_AttackHandler);
        _onRecv.Add((ushort)MsgId.C2SSkill, MakePacket<C2S_Skill>);
        _handler.Add((ushort)MsgId.C2SSkill, PacketHandler.C2S_SkillHandler);
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