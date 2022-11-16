using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using ServerCore;


internal class PacketHandler
{
    public static void C2S_MoveHandler(PacketSession session, IMessage packet)
    {
        C2S_Move? pkt = packet as C2S_Move;
        if (pkt == null) return;
        ClientSession? serverSession = session as ClientSession;
        if(serverSession == null) return;
    }
}
