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
        C2S_Move? req = packet as C2S_Move;
        if (req == null) return;
        ClientSession? clientSession = session as ClientSession;
        if(clientSession == null) return;

        Console.WriteLine($"C2S_Move ({req.PosInfo.PosX}, {req.PosInfo.PosY})");

        if (clientSession.MyPlayer == null)
            return;
        if (clientSession.MyPlayer.Room == null)
            return;

        // TODO : 검증
        PlayerInfo info = clientSession.MyPlayer.Info;
        info.PosInfo = req.PosInfo;

        // 다른 플레이에게 알려준다.
        S2C_Move res = new S2C_Move();
        res.PlayerId = clientSession.MyPlayer.Info.PlayerId;
        res.PosInfo = req.PosInfo;

        clientSession.MyPlayer.Room.Broadcast(res);
    }
}
