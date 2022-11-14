using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DummyClient;
using ServerCore;

class PacketHandler
{
    public static void S2C_BroadcastEnterGameHandler(PacketSession session, IPacket packet)
    {
        S2C_BroadcastEnterGame pkt = new S2C_BroadcastEnterGame();
        ServerSession serverSession = session as ServerSession;
    }
    public static void S2C_BroadcastLeaveGameHandler(PacketSession session, IPacket packet)
    {
        S2C_BroadcastLeaveGame pkt = new S2C_BroadcastLeaveGame();
        ServerSession serverSession = session as ServerSession;
    }
    public static void S2C_PlayerListHandler(PacketSession session, IPacket packet)
    {
        S2C_PlayerList pkt = new S2C_PlayerList();
        ServerSession serverSession = session as ServerSession;
    }
    public static void S2C_BroadcastMoveHandler(PacketSession session, IPacket packet)
    {
        S2C_BroadcastMove pkt = new S2C_BroadcastMove();
        ServerSession serverSession = session as ServerSession;
    }
}
