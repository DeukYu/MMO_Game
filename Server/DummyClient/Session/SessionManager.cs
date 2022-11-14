using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyClient.Session
{
    internal class SessionManager
    {
        static SessionManager _session = new SessionManager();
        public static SessionManager Instance { get { return _session; } }
        List<ServerSession> _sessions = new List<ServerSession>();
        object _lock = new object();   
        Random _rand = new Random();
        public void SendForEach()
        {
            lock (_lock)
            {
                foreach(ServerSession session in _sessions)
                {
                    C2S_Move pkt = new C2S_Move();
                    pkt.posX = _rand.Next(-50, 50);
                    pkt.posY = 0;
                    pkt.posZ = _rand.Next(-50, 50);

                    session.Send(pkt.Write());
                }
            }
        }
        public ServerSession Generate()
        {
            lock (_lock)
            {
                ServerSession session = new ServerSession();
                _sessions.Add(session);
                return session;
            }
        }
    }
}
