using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.Protocol;
using Server.Data;
using Server.DB;

namespace Server.Game
{
    public class Monster : GameObject
    {
        public int TemplateId { get; private set; }
        public Monster()
        {
            ObjectType = GameObjectType.Monster;

            
        }
        public void Init(int templateId)
        {
            TemplateId = templateId;
            MonsterData monsterData = null;
            DataManager.MonsterDict.TryGetValue(TemplateId, out monsterData);
            StatInfo.MergeFrom(monsterData.stat);
            StatInfo.Hp = monsterData.stat.MaxHp;
            State = CreatureState.Idle;
        }

        // FSM (Finite State Machine)
        public override void Update()
        {
            switch (State)
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
        Player _target;
        int _searchCellDist = 10;
        int _chaseCellDist = 20;

        long _nextSearchTick = 0;
        protected virtual void UpdateIdle()
        {
            if (_nextSearchTick > Environment.TickCount64)
                return;
            _nextSearchTick = Environment.TickCount64 + 1000;

            Player target = Room.FindPlayer(p =>
            {
                Vector2Int dir = p.CellPos - CellPos;
                return dir.cellDistFromZero <= _searchCellDist;
            });

            if (target == null)
                return;

            _target = target;
            State = CreatureState.Moving;
        }
        int _attackRange = 1;
        long _nextMoveTick = 0;
        protected virtual void UpdateMoving()
        {
            if (_nextMoveTick > Environment.TickCount64)
                return;

            int moveTick = (int)(1000 / Speed);
            _nextMoveTick = Environment.TickCount64 + moveTick;

            if (_target == null || _target.Room != Room)
            {
                _target = null;
                State = CreatureState.Idle;
                BroadcastMove();
                return;
            }

            Vector2Int dir = _target.CellPos - CellPos;
            int dist = dir.cellDistFromZero;
            if (dist == 0 || dist > _chaseCellDist)
            {
                _target = null;
                State = CreatureState.Idle;
                BroadcastMove();
                return;
            }

            List<Vector2Int> path = Room.Map.FindPath(CellPos, _target.CellPos, false);
            if (path.Count < 2 || path.Count > _chaseCellDist)
            {
                _target = null;
                State = CreatureState.Idle;
                BroadcastMove();
                return;
            }

            // Check Attack 
            if (dist <= _attackRange && (dir.x == 0 || dir.y == 0))
            {
                _coolTick = 0;
                State = CreatureState.Attack;
                return;
            }

            // 이동
            Dir = GetDirFromVec(path[1] - CellPos);
            Room.Map.ApplyMove(this, path[1]);
            BroadcastMove();
        }
        void BroadcastMove()
        {
            S2C_Move movePacket = new S2C_Move();
            movePacket.ObjectId = Id;
            movePacket.PosInfo = PosInfo;
            Room.Broadcast(movePacket);
        }
        long _coolTick = 0;
        protected virtual void UpdateAttack()
        {
            if(_coolTick == 0)
            {
                // 유효한 타겟인지 
                if(_target == null || _target.Room != Room || _target.Hp == 0)
                {
                    _target = null;
                    State = CreatureState.Moving;
                    BroadcastMove();
                    return;
                }
                // 스킬이 아직 사용가능한지
                Vector2Int dir = (_target.CellPos - CellPos);
                int dist = dir.cellDistFromZero;
                bool canUseAttack = (dist <= _attackRange && (dir.x == 0 || dir.y == 0));
                if(canUseAttack == false)
                {
                    State = CreatureState.Moving;
                    BroadcastMove();
                    return;
                }
                if (dist <= _attackRange && (dir.x == 0 || dir.y == 0))
                {
                    _coolTick = 0;
                    State = CreatureState.Attack;
                    return;
                }
                // 타겟팅 방향 주시
                MoveDir lookDir = GetDirFromVec(dir);
                if(lookDir != Dir)
                {
                    Dir = lookDir;
                    BroadcastMove();
                }
                // 데미지 판정
                _target.OnDamaged(this, StatInfo.Attack);

                // 공격 사용 Broadcast
                S2C_Attack attack = new S2C_Attack();
                attack.ObjectId = Id;
                Room.Broadcast(attack);

                // 스킬 쿨타임 적용
                int coolTick = (int)(0.5f * 1000);
                _coolTick = Environment.TickCount64 + coolTick;
            }

            if (_coolTick > Environment.TickCount64)
                return;

            _coolTick = 0;
        }
        protected virtual void UpdateSkill()
        {

        }
        protected virtual void UpdateDead()
        {

        }
        public override void OnDead(GameObject attacker)
        {
            base.OnDead(attacker);

            GameObject owner = attacker.GetOwner();

            if(owner.ObjectType == GameObjectType.Player)
            {
                RewardData rewardData = GetRandomReward();
                if(rewardData != null)
                {
                    Player player = (Player)owner;

                    DbTransaction.RewardPlayer(player, rewardData, Room);
                }
            }
        }
        RewardData GetRandomReward()
        {
            MonsterData monsterData = null;
            DataManager.MonsterDict.TryGetValue(TemplateId, out monsterData);

            int rand = new Random().Next(0, 101);
            int sum = 0;
            foreach(RewardData rewardData in monsterData.rewards)
            {
                sum += rewardData.probability;
                if(rand <= sum)
                {
                    return rewardData;
                }
            }
            return null;
        }
    }
}
