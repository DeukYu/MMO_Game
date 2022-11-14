using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Session;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Google.Protobuf.Protocol.Person.Types;

namespace Server
{
    class ClientSession : PacketSession
    {
        public int SessionId { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            Person person = new Person()
            {
                Id = 1234,
                Name = "John Doe",
                Email = "jdoe@example.com",
                Phones = { new PhoneNumber { Number = "555-4321", Type = Person.Types.PhoneType.Home } }
            };

            ushort size = (ushort)person.CalculateSize();
            byte[] sendBuffer = new byte[size + 4];
            Array.Copy(BitConverter.GetBytes(size + 4), 0, sendBuffer, 0, sizeof(ushort));
            ushort protocolId = 1;
            Array.Copy(BitConverter.GetBytes(protocolId), 0, sendBuffer, 2, sizeof(ushort));
            Array.Copy(person.ToByteArray(), 0, sendBuffer, 4, size);

            Send(new ArraySegment<byte>(sendBuffer));
        }
        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            //PacketManager.Instance.OnRecvPacket(this, buffer);
        }
        public override void OnDisconnected(EndPoint endPoint)
        {
            SessionManager.Instance.Remove(this);

            Console.WriteLine($"OnDisconnected : {endPoint}");
        }
        public override void OnSend(int numOfBytes)
        {
            //Console.WriteLine($"Transferred byte : {numOfBytes}");
        }
    }
}
