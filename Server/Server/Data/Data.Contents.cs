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
        {   // ToDictionary() 사용해보기    
            Dictionary<int, Skill> dict = new Dictionary<int, Skill>();
            foreach (Skill skill in skills)
                dict.Add(skill.id, skill);
            return dict;
        }
    }
    #endregion
}
