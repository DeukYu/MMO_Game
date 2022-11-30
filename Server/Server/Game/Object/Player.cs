using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.Protocol;
using Server.DB;

namespace Server.Game
{
    public class Player : GameObject
    {
        public int PlayerDbId { get; set; }
        public ClientSession? Session { get; set; }
        public Player()
        {
            ObjectType = GameObjectType.Player;
            Speed = 10.0f;
        }
        public override void OnDamaged(GameObject attacker, int damage)
        {
            base.OnDamaged(attacker, damage);
        }
        public override void OnDead(GameObject attacker)
        {
            base.OnDead(attacker);
        }
        public void OnLeaveGame()
        {
            // DB 연동
            using (AppDbContext db = new AppDbContext())
            {
                PlayerDb playerDb = db.Players.Find(PlayerDbId);
                playerDb.Hp = StatInfo.Hp;
                db.SaveChanges();
            }
        }
    }
}
