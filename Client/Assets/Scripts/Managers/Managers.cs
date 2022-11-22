using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_instance;
    static Managers Instance { get { Init(); return s_instance; } }

    #region Contents
    MapManager _map = new MapManager();
    ObjectManager _object = new ObjectManager();
    NetworkManager _network = new NetworkManager();
    public static MapManager Map { get { return Instance._map; } }
    public static ObjectManager Object { get { return Instance._object; } }
    public static NetworkManager Network { get { return Instance._network; } }
    #endregion

    #region Core
    DataManager _data = new DataManager();
    ResourceManager _resource = new ResourceManager();
    PoolManager _pool = new PoolManager();
    SceneManagerEx _scene = new SceneManagerEx();
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static PoolManager Pool { get { return Instance._pool; } }
    public static SceneManagerEx Scene { get { return Instance._scene; } }
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        _network.Update();
    }
    static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
                go.name = "@Managers";
            }
            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();

            s_instance._data.Init();
            s_instance._network.Init();
            s_instance._pool.Init();
        }
    }
    public static void Clear()
    {
        Pool.Clear();
        Scene.Clear();
    }
}
