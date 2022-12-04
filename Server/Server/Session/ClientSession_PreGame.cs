using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DB;
using Server.Game;
using ServerCore;

namespace Server
{
    public partial class ClientSession : PacketSession
    {
        public int AccountDbId { get; private set; }
        public List<LobbyPlayerInfo> LobbyPlayers { get; set; } = new List<LobbyPlayerInfo>();
        public void HandleLogin(C2S_Login loginPacket)
        {
            // TODO : 보안 체크
            if (ServerState != PlayerServerState.ServerStateLogin)
                return;

            LobbyPlayers.Clear();

            using (AppDbContext db = new AppDbContext())
            {
                AccountDb findAccount = db.Accounts
                    .Include(a => a.Players)
                    .Where(a => a.AccountName == loginPacket.UniqueId).FirstOrDefault();

                if (findAccount != null)
                {
                    AccountDbId = findAccount.AccountDbId;

                    S2C_Login res = new S2C_Login() { BSuccess = true };
                    foreach (var playerDb in findAccount.Players)
                    {
                        LobbyPlayerInfo lobbyPlayer = new LobbyPlayerInfo()
                        {
                            PlayerDbId = playerDb.PlayerDbId,
                            Name = playerDb.PlayerName,
                            StatInfo = new StatInfo()
                            {
                                Level = playerDb.Level,
                                Hp = playerDb.Hp,
                                MaxHp = playerDb.MaxHp,
                                Mp = playerDb.Mp,
                                MaxMp = playerDb.MaxMp,
                                AtkSpeed = playerDb.AtkSpeed,
                                Attack = playerDb.Attack,
                                Speed = playerDb.Speed,
                                TotalExp = playerDb.TotalExp
                            }
                        };
                        LobbyPlayers.Add(lobbyPlayer);
                        res.Players.Add(lobbyPlayer);
                    }
                    Send(res);
                    ServerState = PlayerServerState.ServerStateLobby;
                }
                else
                {
                    AccountDb newAccount = new AccountDb() { AccountName = loginPacket.UniqueId };
                    db.Accounts.Add(newAccount);
                    if (db.SaveChangesEx() == false)
                        return;
                    AccountDbId = newAccount.AccountDbId;

                    S2C_Login res = new S2C_Login() { BSuccess = true };
                    Send(res);
                    ServerState = PlayerServerState.ServerStateLobby;
                }
            }
        }
        public void HandleEnterGame(C2S_EnterGame enterGamePacket)
        {
            if (ServerState != PlayerServerState.ServerStateLobby) return;

            LobbyPlayerInfo playerInfo = LobbyPlayers.Find(p => p.Name == enterGamePacket.Name);
            if (playerInfo == null) return;

            MyPlayer = ObjectManager.Instance.Add<Player>();
            {
                MyPlayer.PlayerDbId = playerInfo.PlayerDbId;
                MyPlayer.Info.Name = playerInfo.Name;
                MyPlayer.Info.PosInfo.State = CreatureState.Idle;
                MyPlayer.Info.PosInfo.MoveDir = MoveDir.Down;
                MyPlayer.Info.PosInfo.PosX = 0;
                MyPlayer.Info.PosInfo.PosY = 0;
                MyPlayer.StatInfo.MergeFrom(playerInfo.StatInfo);
                MyPlayer.Session = this;

                S2C_ItemList itemListPacket = new S2C_ItemList();

                using(AppDbContext db = new AppDbContext())
                {
                    List<ItemDb> items = db.Items
                        .Where(i=>i.OwnerDbId == playerInfo.PlayerDbId)
                        .ToList();

                    foreach(ItemDb itemDb in items)
                    {
                        Item item = Item.MakeItem(itemDb);
                        if(item != null)
                        {
                            MyPlayer.Inven.Add(item);
                            ItemInfo info = new ItemInfo();
                            info.MergeFrom(item.Info);
                            itemListPacket.Items.Add(info);
                        }
                    }
                }
                Send(itemListPacket);
            }
            ServerState = PlayerServerState.ServerStateGame;

            GameRoom room = RoomManager.Instance.Find(1);
            if (room == null) return;
            room.Push(room.EnterGame, MyPlayer);
        }
        public void HandleCreatePlayer(C2S_CreatePlayer createPlayerPacket)
        {
            if (ServerState != PlayerServerState.ServerStateLobby) return;

            using (AppDbContext db = new AppDbContext())
            {
                PlayerDb findPlayer = db.Players
                    .Where(p => p.PlayerName == createPlayerPacket.Name).FirstOrDefault();
                if (findPlayer != null)
                    Send(new S2C_CreatePlayer());
                else
                {
                    DataManager.StatDict.TryGetValue(1, out StatInfo? statInfo);

                    PlayerDb newPlayerDb = new PlayerDb()
                    {
                        PlayerName = createPlayerPacket.Name,
                        Level = statInfo.Level,
                        Hp = statInfo.Hp,
                        MaxHp = statInfo.MaxHp,
                        Mp = statInfo.Mp,
                        MaxMp = statInfo.MaxMp,
                        Attack = statInfo.Attack,
                        AtkSpeed = statInfo.AtkSpeed,
                        Speed = statInfo.Speed,
                        TotalExp = 0,
                        AccountDbId = AccountDbId
                    };

                    db.Players.Add(newPlayerDb);
                    if (db.SaveChangesEx() == false)
                        return;

                    // 메모리에 추가
                    LobbyPlayerInfo lobbyPlayer = new LobbyPlayerInfo()
                    {
                        PlayerDbId = newPlayerDb.PlayerDbId,
                        Name = createPlayerPacket.Name,
                        StatInfo = new StatInfo()
                        {
                            Level = statInfo.Level,
                            Hp = statInfo.Hp,
                            MaxHp = statInfo.MaxHp,
                            Mp = statInfo.Mp,
                            MaxMp = statInfo.MaxMp,
                            Attack = statInfo.Attack,
                            AtkSpeed = statInfo.AtkSpeed,
                            Speed = statInfo.Speed,
                            TotalExp = 0
                        }
                    };

                    LobbyPlayers.Add(lobbyPlayer);

                    S2C_CreatePlayer newPlayer = new S2C_CreatePlayer() { Player = new LobbyPlayerInfo() };
                    newPlayer.Player.MergeFrom(lobbyPlayer);
                    Send(newPlayer);
                }
            }
        }
    }
}
