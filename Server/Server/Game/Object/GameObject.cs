using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.Protocol;

namespace Server.Game
{
    public class GameObject
    {
        public GameObjectType ObjectType { get; protected set; } = GameObjectType.None;
        public int Id
        {
            get { return Info.ObjectId; }
            set { Info.ObjectId = value; }
        }
        public GameRoom? Room { get; set; }
        public ObjectInfo Info { get; set; } = new ObjectInfo();
        public PositionInfo PosInfo { get; private set; } = new PositionInfo();
        public StatInfo StatInfo { get; private set; } = new StatInfo();
        public float Speed
        {
            get { return StatInfo.Speed; }
            set { StatInfo.Speed = value; }
        }
        public MoveDir Dir
        {
            get { return PosInfo.MoveDir; }
            set { PosInfo.MoveDir = value; }
        }
        public CreatureState State
        {
            get { return PosInfo.State; }
            set { PosInfo.State = value; }
        }
        public int Hp
        {
            get { return StatInfo.Hp; }
            set { StatInfo.Hp = Math.Clamp(value, 0, StatInfo.MaxHp); }
        }
        public GameObject()
        {
            Info.PosInfo = PosInfo;
            Info.StatInfo = StatInfo;
        }
        public virtual void Update()
        {

        }
        public Vector2Int CellPos
        {
            get
            {
                return new Vector2Int(PosInfo.PosX, PosInfo.PosY);
            }
            set
            {
                PosInfo.PosX = value.x;
                PosInfo.PosY = value.y;
            }
        }
        public Vector2Int GetFrontCellPos()
        {
            return GetFrontCellPos(PosInfo.MoveDir);
        }
        public Vector2Int GetFrontCellPos(MoveDir dir)
        {
            Vector2Int cellPos = CellPos;
            switch (dir)
            {
                case MoveDir.Up:
                    cellPos += Vector2Int.up;
                    break;
                case MoveDir.Down:
                    cellPos += Vector2Int.down;
                    break;
                case MoveDir.Left:
                    cellPos += Vector2Int.left;
                    break;
                case MoveDir.Right:
                    cellPos += Vector2Int.right;
                    break;
            }
            return cellPos;
        }
        public static MoveDir GetDirFromVec(Vector2Int dir)
        {
            if (dir.x > 0)
                return MoveDir.Right;
            else if (dir.x < 0)
                return MoveDir.Left;
            else if (dir.y > 0)
                return MoveDir.Up;
            else
                return MoveDir.Down;
        }
        public virtual void OnDamaged(GameObject attacker, int damage)
        {
            if (Room == null)
                return;

            StatInfo.Hp = Math.Max(StatInfo.Hp - damage, 0);

            S2C_ChangeHp changeHpPacket = new S2C_ChangeHp();
            changeHpPacket.ObjectId = Id;
            changeHpPacket.Hp = StatInfo.Hp;
            Room.Broadcast(changeHpPacket);

            if (StatInfo.Hp <= 0)
            {
                OnDead(attacker);
            }
        }
        public virtual void OnDead(GameObject attacker)
        {
            if (Room == null)
                return;

            S2C_Die diePacket = new S2C_Die();
            diePacket.ObjectId = Id;
            diePacket.AttackerId = attacker.Id;
            Room.Broadcast(diePacket);

            GameRoom room = Room;
            room.LeaveGame(Id);

            StatInfo.Level = StatInfo.MaxHp;
            PosInfo.State = CreatureState.Idle;
            PosInfo.MoveDir = MoveDir.Down;
            PosInfo.PosX = 0;
            PosInfo.PosY = 0;

            room.EnterGame(this);
        }
    }
}
