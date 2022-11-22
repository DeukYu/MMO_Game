using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Data;

namespace Server.Game
{
    public class GameRoom
    {
        object _lock = new object();
        public int RoomId { get; set; }
        Dictionary<int, Player> _players = new Dictionary<int, Player>();
        Dictionary<int, Monster> _monsters = new Dictionary<int, Monster>();
        Dictionary<int, Projectile> _projectile = new Dictionary<int, Projectile>();

        public Map Map { get; private set; } = new Map();
        public void Init(int mapId)
        {
            Map.LoadMap(mapId);
        }
        public void Update()
        {
            lock (_lock)
            {
                foreach (Projectile projectile in _projectile.Values)
                {
                    projectile.Update();
                }
            }
        }
        public void EnterGame(GameObject gameObject)
        {
            if (gameObject == null)
                return;

            GameObjectType type = ObjectManager.GetObjectTypeById(gameObject.Id);

            lock (_lock)
            {
                if (type == GameObjectType.Player)
                {
                    Player? player = gameObject as Player;
                    _players.Add(gameObject.Id, player);
                    player.Room = this;

                    // 본인
                    {
                        S2C_EnterGame enterPkt = new S2C_EnterGame();
                        enterPkt.Player = player.Info;
                        player.Session.Send(enterPkt);

                        S2C_Spawn spawnPkt = new S2C_Spawn();
                        foreach (Player p in _players.Values)
                        {
                            if (player != p)
                                spawnPkt.Objects.Add(p.Info);
                        }
                        player.Session.Send(spawnPkt);
                    }
                }
                else if (type == GameObjectType.Monster)
                {
                    Monster? monster = gameObject as Monster;
                    _monsters.Add(gameObject.Id, monster);
                    monster.Room = this;
                }
                else if (type == GameObjectType.Projectile)
                {
                    Projectile? projectile = gameObject as Projectile;
                    _projectile.Add(gameObject.Id, projectile);
                    projectile.Room = this;
                }
                // 타인
                {
                    S2C_Spawn spawnPkt = new S2C_Spawn();
                    spawnPkt.Objects.Add(gameObject.Info);
                    foreach (Player p in _players.Values)
                    {
                        if (p.Id != gameObject.Id)
                            p.Session.Send(spawnPkt);
                    }
                }
            }
        }
        public void LeaveGame(int objectId)
        {
            GameObjectType type = ObjectManager.GetObjectTypeById(objectId);

            lock (_lock)
            {
                if (type == GameObjectType.Player)
                {
                    Player? player = null;
                    if (_players.Remove(objectId, out player) == false)
                        return;

                    player.Room = null;
                    Map.ApplyLeave(player);

                    // 본인
                    {
                        S2C_LeaveGame leavePkt = new S2C_LeaveGame();
                        player.Session.Send(leavePkt);
                    }
                }
                else if (type == GameObjectType.Monster)
                {
                    Monster? monster = null;
                    if (_monsters.Remove(objectId, out monster) == false)
                        return;

                    monster.Room = null;
                    Map.ApplyLeave(monster);
                }
                else if (type == GameObjectType.Projectile)
                {
                    Projectile? projectile = null;
                    if (_projectile.Remove(objectId, out projectile) == false)
                        return;

                    projectile.Room = null;
                }

                // 타인
                {
                    S2C_Despawn despawnPkt = new S2C_Despawn();
                    despawnPkt.ObjectIds.Add(objectId);
                    foreach (Player p in _players.Values)
                    {
                        if (p.Id != objectId)
                            p.Session.Send(despawnPkt);
                    }
                }
            }
        }
        public void HandleMove(Player player, C2S_Move movePacket)
        {
            if (player == null)
                return;

            lock (_lock)
            {
                PositionInfo? movePosInfo = movePacket.PosInfo;
                ObjectInfo? info = player.Info;

                // 다른 좌표로 이동할 경우, 갈 수 있는지 체크
                if (movePosInfo.PosX != info.PosInfo.PosX || movePosInfo.PosY != info.PosInfo.PosY)
                {
                    if (Map.CanGo(new Vector2Int(movePosInfo.PosX, movePosInfo.PosY)) == false)
                        return;
                }

                info.PosInfo.State = movePosInfo.State;
                info.PosInfo.MoveDir = movePosInfo.MoveDir;
                Map.ApplyMove(player, new Vector2Int(movePosInfo.PosX, movePosInfo.PosY));

                // 다른 플레이에게 알려준다.
                S2C_Move res = new S2C_Move();
                res.ObjectId = player.Info.ObjectId;
                res.PosInfo = movePacket.PosInfo;

                Broadcast(res);
            }
        }
        public void HandleAttack(Player player, C2S_Attack attackPacket)
        {
            if (player == null)
                return;

            lock (_lock)
            {
                ObjectInfo? info = player.Info;
                if (info.PosInfo.State != CreatureState.Idle)
                    return;

                info.PosInfo.State = CreatureState.Attack;
                S2C_Attack res = new S2C_Attack() { };
                res.ObjectId = info.ObjectId;
                Broadcast(res);

                // 데미지 판정
                Vector2Int attackPos = player.GetFrontCellPos(info.PosInfo.MoveDir);
                GameObject target = Map.Find(attackPos);
                if (target != null)
                {
                    Console.WriteLine("Hit GameObject !");
                }
            }
        }
        public void HandleSkill(Player player, C2S_Skill skillPacket)
        {
            if (player == null)
                return;

            lock (_lock)
            {
                ObjectInfo? info = player.Info;
                if (info.PosInfo.State != CreatureState.Idle)
                    return;

                info.PosInfo.State = CreatureState.Skill;
                S2C_Skill res = new S2C_Skill() { Info = new SkillInfo() };
                res.ObjectId = info.ObjectId;
                res.Info.SkillId = skillPacket.Info.SkillId;
                Broadcast(res);

                Data.Skill skillData = null;
                if (DataManager.SkillDict.TryGetValue(skillPacket.Info.SkillId, out skillData) == false)
                    return;

                switch (skillData.skillType)
                {
                    case SkillType.SkillAuto:
                        {

                        }
                        break;
                    case SkillType.SkillProjectile:
                        {
                            Arrow arrow = ObjectManager.Instance.Add<Arrow>();
                            if (arrow == null)
                                return;

                            arrow.Owner = player;
                            arrow.Data = skillData;

                            arrow.PosInfo.State = CreatureState.Moving;
                            arrow.PosInfo.MoveDir = player.PosInfo.MoveDir;
                            arrow.PosInfo.PosX = player.PosInfo.PosX;
                            arrow.PosInfo.PosY = player.PosInfo.PosY;
                            arrow.Speed = skillData.projectileInfo.speed;
                            EnterGame(arrow);
                        }
                        break;
                }
            }
        }
        public void Broadcast(IMessage packet)
        {
            lock (_lock)
            {
                foreach (Player p in _players.Values)
                {
                    if (p.Session == null)
                        continue;
                    p.Session.Send(packet);
                }
            }
        }
    }
}
