using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Data
{
    public interface ILoader<Key, Value>
    {
        Dictionary<Key, Value> MakeDict();
    }
    public class DataManager
    {
        public static Dictionary<int, Data.Stat>? StatDict { get; private set; }
        public static Dictionary<int, Data.Skill>? SkillDict { get; private set; }
        public static void LoadData()
        {
            StatDict = LoadJson<Data.StatData, int, Data.Stat>("StatData").MakeDict();
            SkillDict = LoadJson<Data.SkillData, int, Data.Skill>("SkillData").MakeDict();
        }
        static Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
        {
            string text = File.ReadAllText($"{ConfigManager.Config.dataPath}/{path}.json");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Loader>(text);
        }
    }

}
