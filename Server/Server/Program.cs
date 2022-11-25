using System.Net;
using Server.Data;
using Server.Game;
using Server.Session;
using ServerCore;

namespace Server
{
    class Program
    {
        static Listener _listener = new Listener();
        static void Main(string[] args)
        {
            ConfigManager.LoadConfig();
            DataManager.LoadData();

            var d = DataManager.StatDict;

            RoomManager.Instance.Add(1);

            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
            Console.WriteLine("Listening...");

            // TODO
            while (true)
            {
                GameRoom room = RoomManager.Instance.Find(1);
                room.Push(room.Update);
                Thread.Sleep(100);
            }
        }
    }
}