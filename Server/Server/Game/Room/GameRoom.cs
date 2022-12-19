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
    public class GameRoom : JobSerializer
    {
        public int RoomId { get; set; }
        Dictionary<int, Player> _players = new Dictionary<int, Player>();
        Dictionary<int, Monster> _monsters = new Dictionary<int, Monster>();
        Dictionary<int, Projectile> _projectile = new Dictionary<int, Projectile>();
        public Map Map { get; private set; } = new Map();
        public void Init(int mapId)
        {
            Map.LoadMap(mapId);

            // TEMP
            Monster monster = ObjectManager.Instance.Add<Monster>();
            monster.Init(1);
            monster.CellPos = new Vector2Int(5, 5);
            EnterGame(monster);
        }
        public void Update()
        {
            foreach (Monster monster in _monsters.Values)
            {
                monster.Update();
            }
            foreach (Projectile projectile in _projectile.Values)
            {
                projectile.Update();
            }
            Flush();
        }
        public void EnterGame(GameObject gameObject)
        {
            if (gameObject == null)
                return;

            GameObjectType type = ObjectManager.GetObjectTypeById(gameObject.Id);

            if (type == GameObjectType.Player)
            {
                Player? player = gameObject as Player;
                _players.Add(gameObject.Id, player);
                player.Room = this;

                Map.ApplyMove(player, new Vector2Int(player.CellPos.x, player.CellPos.y));
                // 본인
                {
                    S2C_EnterGame Res_EnterPkt = new S2C_EnterGame();
                    Res_EnterPkt.Player = player.Info;
                    if (player.Session != null)
                        player.Session.Send(Res_EnterPkt);

                    S2C_Spawn Res_SpawnPkt = new S2C_Spawn();
                    foreach (Player p in _players.Values)
                    {
                        if (player != p)
                            Res_SpawnPkt.Objects.Add(p.Info);
                    }
                    foreach (Monster m in _monsters.Values)
                    {
                        Res_SpawnPkt.Objects.Add(m.Info);
                    }
                    foreach (Projectile p in _projectile.Values)
                    {
                        Res_SpawnPkt.Objects.Add(p.Info);
                    }
                    if (player.Session != null)
                        player.Session.Send(Res_SpawnPkt);
                }
            }
            else if (type == GameObjectType.Monster)
            {
                Monster? monster = gameObject as Monster;
                _monsters.Add(gameObject.Id, monster);
                monster.Room = this;

                Map.ApplyMove(monster, new Vector2Int(monster.CellPos.x, monster.CellPos.y));
            }
            else if (type == GameObjectType.Projectile)
            {
                Projectile? projectile = gameObject as Projectile;
                _projectile.Add(gameObject.Id, projectile);
                projectile.Room = this;
            }
            // 타인
            {
                S2C_Spawn Res_SpawnPkt = new S2C_Spawn();
                Res_SpawnPkt.Objects.Add(gameObject.Info);
                foreach (Player p in _players.Values)
                {
                    if (p.Id != gameObject.Id)
                        if (p.Session != null)
                            p.Session.Send(Res_SpawnPkt);
                }
            }
        }
        public void LeaveGame(int objectId)
        {
            GameObjectType type = ObjectManager.GetObjectTypeById(objectId);
            if (type == GameObjectType.Player)
            {
                Player? player = null;
                if (_players.Remove(objectId, out player) == false)
                    return;

                player.OnLeaveGame();
                Map.ApplyLeave(player);
                player.Room = null;

                // 본인
                {
                    S2C_LeaveGame Res_LeavePkt = new S2C_LeaveGame();
                    if (player.Session != null)
                        player.Session.Send(Res_LeavePkt);
                }
            }
            else if (type == GameObjectType.Monster)
            {
                Monster? monster = null;
                if (_monsters.Remove(objectId, out monster) == false)
                    return;

                Map.ApplyLeave(monster);
                monster.Room = null;
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
                S2C_Despawn Res_DespawnPkt = new S2C_Despawn();
                Res_DespawnPkt.ObjectIds.Add(objectId);
                foreach (Player p in _players.Values)
                {
                    if (p.Id != objectId)
                        if (p.Session != null)
                            p.Session.Send(Res_DespawnPkt);
                }
            }
        }
        public void HandleMove(Player player, C2S_Move movePacket)
        {
            if (player == null)
                return;

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
            S2C_Move Res_MovePkt = new S2C_Move();
            Res_MovePkt.ObjectId = player.Info.ObjectId;
            Res_MovePkt.PosInfo = movePacket.PosInfo;
            Broadcast(Res_MovePkt);
        }
        public void HandleAttack(Player player, C2S_Attack attackPacket)
        {
            if (player == null) return;

            ObjectInfo? info = player.Info;
            if (info.PosInfo.State != CreatureState.Idle) return;

            info.PosInfo.State = CreatureState.Attack;
            S2C_Attack Res_AttackPkt = new S2C_Attack() { };
            Res_AttackPkt.ObjectId = info.ObjectId;
            Broadcast(Res_AttackPkt);

            // 데미지 판정
            Vector2Int attackPos = player.GetFrontCellPos(info.PosInfo.MoveDir);
            GameObject target = Map.Find(attackPos);
            if (target != null)
            {
                Console.WriteLine("Hit GameObject !");
            }
        }
        public void HandleSkill(Player player, C2S_Skill skillPacket)
        {
            if (player == null)
                return;

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
                        Push(EnterGame, arrow);
                    }
                    break;
            }
        }
        public Player? FindPlayer(Func<GameObject, bool> condition)
        {
            foreach (Player player in _players.Values)
            {
                if (condition.Invoke(player))
                    return player;
            }
            return null;
        }
        public void Broadcast(IMessage packet)
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
