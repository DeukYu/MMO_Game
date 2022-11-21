using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.Protocol;

namespace Server.Game
{
    public class Arrow : Projectile
    {
        public GameObject? Owner { get; set; }
        public void Update()
        {

        }
    }
}
