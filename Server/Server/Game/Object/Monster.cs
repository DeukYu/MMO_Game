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

            // TEMP
            StatInfo.Level = 1;
            StatInfo.Hp = 100;
            StatInfo.MaxHp = 100;
            StatInfo.Speed = 5.0f;

            State = CreatureState.Idle;
        }

        // FSM (Finite State Machine)
        public override void Update()
        {
            switch(State)
            {
                case CreatureState.Idle:
                    UpdateIdle();
                    break;
                case CreatureState.Moving:
                    UpdateMoving();
                    break;
                case CreatureState.Attack:
                    UpdateAttack();
                    break;
                case CreatureState.Skill:
                    UpdateSkill();
                    break;
                case CreatureState.Dead:
                    UpdateDead();
                    break;
            }
        }
        long _nextSearchTick = 0;
        protected virtual void UpdateIdle()
        {
            if (_nextSearchTick > Environment.TickCount64)
                return;
            _nextSearchTick = Environment.TickCount64 + 1000;

            Room.FindPlayer(p =>
            {
                Vector2Int dir = p.CellPos - CellPos;
                return true;
            });
        }
        protected virtual void UpdateMoving()
        {

        }
        protected virtual void UpdateAttack()
        {

        }
        protected virtual void UpdateSkill()
        {

        }
        protected virtual void UpdateDead()
        {

        }
    }
}
