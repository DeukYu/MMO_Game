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
        long _nextMoveTick = 0;
        public override void Update()
        {
            if (Data == null|| Data.projectileInfo == null|| Owner == null || Room == null)
                return;

            if (_nextMoveTick >= Environment.TickCount64)
                return;

            long tick = (long)(1000 / Data.projectileInfo.speed);
            _nextMoveTick = Environment.TickCount64 + tick;

            Vector2Int destPos = GetFrontCellPos();
            if(Room.Map.CanGo(destPos))
            {
                CellPos = destPos;

                S2C_Move movePacket = new S2C_Move();
                movePacket.ObjectId = Id;
                movePacket.PosInfo = PosInfo;
                Room.Broadcast(movePacket);

                Console.WriteLine("Move Arrow");
            }
            else
            {
                GameObject target = Room.Map.Find(destPos);
                if(target != null)
                {
                    // TODO : 피격 판정
                }
                Room.LeaveGame(Id);
            }
        }
    }
}
