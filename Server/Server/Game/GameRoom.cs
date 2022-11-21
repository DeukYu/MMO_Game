using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.Protocol;

namespace Server.Game
{
    public class GameRoom
    {
        object _lock = new object();
        public int RoomId { get; set; }
        Dictionary<int, Player> _players = new Dictionary<int, Player>();

        Map _map = new Map();
        public void Init(int mapId)
        {
            _map.LoadMap(mapId);
        }
        
        public void EnterGame(Player newPlayer)
        {
            if (newPlayer == null)
                return;

            lock (_lock)
            {
                _players.Add(newPlayer.Info.PlayerId, newPlayer);
                newPlayer.Room = this;

                // 본인
                {
                    S2C_EnterGame enterPkt = new S2C_EnterGame();
                    enterPkt.Player = newPlayer.Info;
                    newPlayer.Session.Send(enterPkt);

                    S2C_Spawn spawnPkt = new S2C_Spawn();
                    foreach (Player p in _players.Values)
                    {
                        if (newPlayer != p)
                            spawnPkt.Players.Add(p.Info);
                    }
                    newPlayer.Session.Send(spawnPkt);
                }
                // 타인
                {
                    S2C_Spawn spawnPkt = new S2C_Spawn();
                    spawnPkt.Players.Add(newPlayer.Info);
                    foreach (Player p in _players.Values)
                    {
                        if (newPlayer != p)
                            p.Session.Send(spawnPkt);
                    }
                }
            }
        }
        public void LeaveGame(int playerId)
        {
            lock (_lock)
            {
                Player? player = null;
                if (_players.Remove(playerId, out player) == false)
                    return;

                player.Room = null;

                // 본인
                {
                    S2C_LeaveGame leavePkt = new S2C_LeaveGame();
                    player.Session.Send(leavePkt);
                }
                // 타인
                {
                    S2C_Despawn despawnPkt = new S2C_Despawn();
                    despawnPkt.PlayerIds.Add(player.Info.PlayerId);
                    foreach (Player p in _players.Values)
                    {
                        if (player != p)
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
                PlayerInfo? info = player.Info;

                // 다른 좌표로 이동할 경우, 갈 수 있는지 체크
                if(movePosInfo.PosX != info.PosInfo.PosX || movePosInfo.PosY != info.PosInfo.PosY)
                {
                    if (_map.CanGo(new Vector2Int(movePosInfo.PosX, movePosInfo.PosY)) == false)
                        return;
                }

                info.PosInfo.State = movePosInfo.State;
                info.PosInfo.MoveDir = movePosInfo.MoveDir;
                _map.ApplyMove(player, new Vector2Int(movePosInfo.PosX, movePosInfo.PosY));

                // 다른 플레이에게 알려준다.
                S2C_Move res = new S2C_Move();
                res.PlayerId = player.Info.PlayerId;
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
                PlayerInfo? info = player.Info;
                if (info.PosInfo.State != CreatureState.Idle)
                    return;

                // TODO : 공격 사용 가능 여부 체크

                info.PosInfo.State = CreatureState.Attack;
                S2C_Attack res = new S2C_Attack() { };
                res.PlayerId = info.PlayerId;
                Broadcast(res);

                // 데미지 판정
                Vector2Int attackPos = player.GetFrontCellPos(info.PosInfo.MoveDir);
                Player target = _map.Find(attackPos);
                if(target != null)
                {
                    Console.WriteLine("Hit Player !");
                }
            }
        }
        public void HandleSkill(Player player, C2S_Skill skillPacket)
        {
            if (player == null)
                return;

            lock (_lock)
            {
                PlayerInfo? info = player.Info;
                if (info.PosInfo.State != CreatureState.Idle)
                    return;

                // TODO : 스킬 사용 가능 여부 체크

                info.PosInfo.State = CreatureState.Skill;
                S2C_Skill res = new S2C_Skill() { Info = new SkillInfo() };
                res.PlayerId = info.PlayerId;
                res.Info.SkillId = 1;
                Broadcast(res);

                // TODO : 데미지 판정
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
