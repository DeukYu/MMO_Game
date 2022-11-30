using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Data;
using Server.Game;
using Server.Session;
using ServerCore;
using System.Net;

namespace Server
{
    public partial class ClientSession : PacketSession
    {
        public PlayerServerState ServerState { get; private set; } = PlayerServerState.ServerStateLogin;
        public Player? MyPlayer { get; set; }
        public int SessionId { get; set; }
        #region Network 
        public void Send(IMessage packet)
        {
            string msgName = packet.Descriptor.Name.Replace("_", string.Empty);
            MsgId msgId = (MsgId)Enum.Parse(typeof(MsgId), msgName);
            ushort size = (ushort)packet.CalculateSize();
            byte[] sendBuffer = new byte[size + 4];
            Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
            Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, 2, sizeof(ushort));
            Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);
            Send(new ArraySegment<byte>(sendBuffer));
        }
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            S2C_Connected connectedPacket = new S2C_Connected();
            Send(connectedPacket);
        }
        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }
        public override void OnDisconnected(EndPoint endPoint)
        {
            GameRoom? room = RoomManager.Instance.Find(1);
            if (room != null)
                room.Push(room.LeaveGame, MyPlayer.Info.ObjectId);

            SessionManager.Instance.Remove(this);

            Console.WriteLine($"OnDisconnected : {endPoint}");
        }
        public override void OnSend(int numOfBytes)
        {
            //Console.WriteLine($"Transferred byte : {numOfBytes}");
        }
        #endregion
    }
}
