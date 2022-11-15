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
    public static void C2S_ChatHandler(PacketSession session, IMessage packet)
    {
        S2C_Chat? pkt = packet as S2C_Chat;
        if (pkt == null) return;
        ClientSession? serverSession = session as ClientSession;
        if(serverSession == null) return;

        Console.WriteLine(pkt.Context);
    }
}
