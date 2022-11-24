using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.Protocol;

namespace Server.Game
{
    public class ObjectManager
    {
        public static ObjectManager Instance { get; } = new ObjectManager();
        object _lock = new object();
        Dictionary<int, Player> _players = new Dictionary<int, Player>();
        Dictionary<int, Monster> _monsters = new Dictionary<int, Monster>();

        // [UNUSED(1)][TYPE(7)][ID(24)]
        int _counter = 0;

        public T Add<T>() where T : GameObject, new()
        {
            T gameObject = new T();

            lock (_lock)
            {
                gameObject.Id = GnerateId(gameObject.ObjectType);
                
                if (gameObject.ObjectType == GameObjectType.Player)
                {
                    _players.Add(gameObject.Id, gameObject as Player);
                }
                else if(gameObject.ObjectType == GameObjectType.Monster)
                {
                    _monsters.Add(gameObject.Id, gameObject as Monster);
                }
            }
            return gameObject;
        }
        int GnerateId(GameObjectType type)
        {
            lock (_lock)
            {
                return (int)type << 24 | (_counter++);
            }
        }
        public static GameObjectType GetObjectTypeById(int id)
        {
            int type = (id >> 24) & 0x7F;
            return (GameObjectType)type;
        }
        public bool Remove(int objectId)
        {
            GameObjectType objectType = GetObjectTypeById(objectId);
            lock (_lock)
            {
                if (objectType == GameObjectType.Player)
                    return _players.Remove(objectId);
                else if(objectType == GameObjectType.Monster)
                    return _monsters.Remove(objectId);
            }
            return false;
        }
        public GameObject? Find(int objectId)
        {
            GameObjectType objectType = GetObjectTypeById(objectId);
            lock (_lock)
            {
                if (objectType == GameObjectType.Player)
                {
                    if (_players.TryGetValue(objectId, out Player? player))
                        return player;
                }
                else if (objectType == GameObjectType.Monster)
                {
                    if (_monsters.TryGetValue(objectId, out Monster? monster))
                        return monster;
                }
                return null;
            }
        }
    }
}
