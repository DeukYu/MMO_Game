using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DB
{
    [Table("Account")]
    public class AccountDb
    {
        public int AccountDbId { get; set; }
        public string AccountName { get;set; }
        public ICollection<PlayerDb> Players { get; set; }
    }
    [Table("Player")]
    public class PlayerDb
    {
        public int PlayerDbId { get; set; }
        public string PlayerName { get; set; }
        [ForeignKey("Account")]
        public int AccountDbId { get; set; }
        public AccountDb Account { get; set; }

        public int Level { get; set; }
        public int Hp { get; set; }
        public int MaxHp { get; set; }
        public int Mp { get; set; }
        public int MaxMp { get; set; }
        public int Attack { get; set; }
        public float AtkSpeed { get; set; }
        public float Speed { get; set; }
        public int TotalExp { get; set; }
    }
}
