using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.Protocol;

namespace Server.Game
{
    public class Player : GameObject
    {
        object _lock = new object();
        public ClientSession? Session { get; set; }
        public Player()
        {
            ObjectType = GameObjectType.Player;
            Speed = 10.0f;
        }
        public override void OnDamaged(GameObject attacker, int damage)
        {
            Console.WriteLine($"TODO : damage {damage}");
        }
    }
}
