using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.Protocol;

namespace Server.Game
{
    public class GameRoom
    {
        object _lock = new object();
        public int RoomId { get; set; }
        List<Player> _players = new List<Player>();
        public void EnterGame(Player newPlayer)
        {
            if (newPlayer == null)
                return;

            lock (_lock)
            {
                _players.Add(newPlayer);
                newPlayer.Room = this;

                // 본인
                {
                    S2C_EnterGame enterPkt = new S2C_EnterGame();
                    enterPkt.Player = newPlayer.Info;
                    newPlayer.Session.Send(enterPkt);

                    S2C_Spawn spawnPkt = new S2C_Spawn();
                    foreach (Player p in _players)
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
                    foreach (Player p in _players)
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
                Player? player = _players.Find(match: p => p.Info.PlayerId == playerId);
                if (player == null)
                    return;

                _players.Remove(player);
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
                    foreach (Player p in _players)
                    {
                        if (player != p)
                            p.Session.Send(despawnPkt);
                    }
                }
            }
        }
    }
}
