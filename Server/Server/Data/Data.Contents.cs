using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.Protocol;

namespace Server.Data
{
    #region Stat
    [Serializable]
    public class StatData : ILoader<int, StatInfo>
    {
        public List<StatInfo> stats = new List<StatInfo>();
        public Dictionary<int, StatInfo> MakeDict()
        {   // ToDictionary() 사용해보기    
            Dictionary<int, StatInfo> dict = new Dictionary<int, StatInfo>();
            foreach (StatInfo stat in stats)
            {
                stat.Hp = stat.MaxHp;
                stat.Mp = stat.MaxMp;
                dict.Add(stat.Level, stat);
            }       
            return dict;
        }
    }
    #endregion
    #region Skill
    [Serializable]
    public class Skill
    {
        public int id;
        public string? name;
        public float cooldown;
        public int damage;
        public SkillType skillType;
        public ProjectileInfo? projectileInfo;
    }
    public class ProjectileInfo
    {
        public string? name;
        public float speed;
        public int range;
        public string? prefab;
    }
    [Serializable]
    public class SkillData : ILoader<int, Skill>
    {
        public List<Skill> skills = new List<Skill>();
        public Dictionary<int, Skill> MakeDict()
        {      
            Dictionary<int, Skill> dict = new Dictionary<int, Skill>();
            foreach (Skill skill in skills)
                dict.Add(skill.id, skill);
            return dict;
        }
    }
    #endregion
    #region Item
    [Serializable]
    public class ItemData
    {
        public int id;
        public string? name;
        public ItemType itemType;
    }

    public class WeaponData : ItemData
    {
        public WeaponType weaponType;
        public int damage;
    }

    public class ArmorData : ItemData
    {
        public ArmorType armorType;
        public int defence;
    }

    public class ConsumableData : ItemData
    {
        public ConsumableType consumableType;
        public int maxCount;
    }

    [Serializable]
    public class ItemLoader : ILoader<int, ItemData>
    {
        public List<WeaponData> weapons = new List<WeaponData>();
        public List<ArmorData> armors = new List<ArmorData>();
        public List<ConsumableData> consumables = new List<ConsumableData>();
        public Dictionary<int, ItemData> MakeDict()
        {
            Dictionary<int, ItemData> dict = new Dictionary<int, ItemData>();
            foreach (ItemData item in weapons)
            {
                item.itemType = ItemType.Weapon;
                dict.Add(item.id, item);
            }
            foreach (ItemData item in armors)
            {
                item.itemType = ItemType.Armor;
                dict.Add(item.id, item);
            }
            foreach (ItemData item in consumables)
            {
                item.itemType = ItemType.Consumable;
                dict.Add(item.id, item);
            }
            return dict;
        }
    }
    #endregion
}
