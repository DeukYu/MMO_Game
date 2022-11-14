using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using ServerCore;

namespace Server
{
    internal class PacketHandler
    {
		public static void C2S_LeaveGameHandler(PacketSession session, IMessage packet)
		{
			//ClientSession? clientSession = session as ClientSession;

			//if (clientSession.Room == null)
			//	return;

			//GameRoom room = clientSession.Room;
			//room.Push(() => room.Leave(clientSession));
		}
		public static void C2S_MoveHandler(PacketSession session, IMessage packet)
		{
			//C2S_Move pkt = packet as C2S_Move;
			//ClientSession? clientSession = session as ClientSession;

			//if (clientSession.Room == null)
			//	return;

			////Console.WriteLine($"{pkt.posX}, {pkt.posY}, {pkt.posZ}");

			//GameRoom room = clientSession.Room;
			//room.Push(() => room.Move(clientSession, pkt));
		}
	}
}
