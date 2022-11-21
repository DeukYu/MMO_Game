using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.Protocol;

namespace Server.Game
{
    internal class Monster : GameObject
    {
        public Monster()
        {
            ObjectType = GameObjectType.Monster;
        }
    }
}
