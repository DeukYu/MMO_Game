using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Session
{
    internal class SessionManager
    {
        static SessionManager _session = new SessionManager();
        public static SessionManager Instance { get { return _session; } }

        int _sessionId = 0;
        Dictionary<int, ClientSession> _sessions = new Dictionary<int, ClientSession>();
        object _lock = new object();

        public ClientSession Generate()
        {
            lock (_lock)
            {
                int sessionId = ++_sessionId;
                ClientSession session = new ClientSession();
                session.SessionId = sessionId;

                Console.WriteLine($"Connected : {sessionId}");
                return session;
            }
        }
        public ClientSession Find(int id)
        {
            lock (_lock)
            {
                ClientSession? session = null;
                _sessions.TryGetValue(id, out session);
#pragma warning disable CS8603 // 가능한 null 참조 반환입니다.
                return session;
#pragma warning restore CS8603 // 가능한 null 참조 반환입니다.
            }
        }
        public void Remove(ClientSession session)
        {
            lock (_lock)
            {
                _sessions.Remove(session.SessionId);
            }  
        }
    }
}
