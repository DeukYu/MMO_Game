using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PacketHandler
{
	public static void S2C_ChatHandler(PacketSession session, IMessage packet)
	{
		S2C_Chat chatPacket = packet as S2C_Chat;
		ServerSession serverSession = session as ServerSession;

		Debug.Log(chatPacket.Context);
	}

	public static void S2C_EnterGameHandler(PacketSession session, IMessage packet)
	{
		S2C_EnterGame enterGamePacket = packet as S2C_EnterGame;
		ServerSession serverSession = session as ServerSession;
	}
}
