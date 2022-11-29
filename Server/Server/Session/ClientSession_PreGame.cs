using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using Server.DB;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public partial class ClientSession : PacketSession
    {
        public void HandleLogin(C2S_Login loginPacket)
        {
            // TODO : 보안 체크
            if (ServerState != PlayerServerState.ServerStateLogin)
                return;
            // TODO : Problem
            using (AppDbContext db = new AppDbContext())
            {
                AccountDb findAccount = db.Accounts
                    .Include(a => a.Players)
                    .Where(a => a.AccountName == loginPacket.UniqueId).FirstOrDefault();

                if (findAccount != null)
                {
                    S2C_Login res = new S2C_Login() { BSuccess = true };
                    Send(res);
                }
                else
                {
                    AccountDb newAccount = new AccountDb() { AccountName = loginPacket.UniqueId };
                    db.Accounts.Add(newAccount);
                    db.SaveChanges();

                    S2C_Login res = new S2C_Login() { BSuccess = true };
                    Send(res);
                }
            }
        }
    }
}
