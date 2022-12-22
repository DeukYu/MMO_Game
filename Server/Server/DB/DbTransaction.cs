using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DB
{
    public class DbTransaction : JobSerializer
    {
        public static DbTransaction Instance { get; } = new DbTransaction();
        public static void SavePlayerStatus_AllInOne(Player player, GameRoom room)
        {
            if (player == null || room == null)
                return;

            PlayerDb playerDb = new PlayerDb();
            playerDb.PlayerDbId = player.PlayerDbId;
            playerDb.Hp = player.StatInfo.Hp;

            Instance.Push(() =>
            {
                using (AppDbContext db = new AppDbContext())
                {
                    db.Entry(playerDb).State = EntityState.Unchanged;
                    db.Entry(playerDb).Property(nameof(playerDb.Hp)).IsModified = true;
                    if (db.SaveChangesEx())
                    {
                        room.Push(() => Console.WriteLine($"Hp Saved{playerDb.Hp}"));
                    }
                }
            });
        }

        public static void SavePlayerStatus_AllInOne_Step1(Player player, GameRoom room)
        {
            if (player == null || room == null)
                return;

            PlayerDb playerDb = new PlayerDb();
            playerDb.PlayerDbId = player.PlayerDbId;
            playerDb.Hp = player.StatInfo.Hp;
            Instance.Push<PlayerDb, GameRoom>(SavePlayerSatus_Step2, playerDb, room);
        }

        public static void SavePlayerSatus_Step2(PlayerDb playerDb, GameRoom room)
        {
            using (AppDbContext db = new AppDbContext())
            {
                db.Entry(playerDb).State = EntityState.Unchanged;
                db.Entry(playerDb).Property(nameof(playerDb.Hp)).IsModified = true;
                if (db.SaveChangesEx())
                {
                    room.Push(SavePlayerSatus_Step3, playerDb.Hp);
                }
            }
        }
        public static void SavePlayerSatus_Step3(int hp)
        {
            Console.WriteLine($"Hp Saved{hp}");
        }
        public static void RewardPlayer(Player player, RewardData rewardData, GameRoom room)
        {
            if (player == null || room == null || rewardData == null)
                return;

            // TODO : Problem
            int? slot = player.Inven.GetEmptySlot();
            if (slot == null)
                return;

            ItemDb itemDb = new ItemDb()
            {
                TemplateId = rewardData.itemId,
                Count = rewardData.count,
                Slot = slot.Value,
                OwnerDbId = player.PlayerDbId
            };

            Instance.Push(() =>
            {
                using (AppDbContext db = new AppDbContext())
                {
                    db.Items.Add(itemDb);
                    if (db.SaveChangesEx())
                    {
                        room.Push(() => 
                        {
                            Item newItem = Item.MakeItem(itemDb);
                            player.Inven.Add(newItem);

                            // TODO : Client Noti
                            {
                                S2C_AddItem itemPacket = new S2C_AddItem();
                                ItemInfo itemInfo = new ItemInfo();
                                itemInfo.MergeFrom(newItem.Info);
                                itemPacket.Items.Add(itemInfo);

                                player.Session.Send(itemPacket);
                            }
                        });
                    }
                }
            });
        }
    }
}
