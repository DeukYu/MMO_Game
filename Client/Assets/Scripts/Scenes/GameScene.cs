using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Game;

        Managers.Map.LoadMap(1);

        //GameObject player = Managers.Resource.Instantiate("Creature/Player");
        //player.name = "Player";
        //Managers.Object.Add(player);

        //for(int i=0;i<5;i++)
        //{
        //    GameObject monster = Managers.Resource.Instantiate("Creature/Monster");
        //    monster.name = $"Monster_{i + 1}";

        //    // 랜덤 위치 스폰
        //    Vector3Int pos = new Vector3Int()
        //    {
        //        x = Random.Range(-20, 20),
        //        y = Random.Range(-10, 10),
        //    };
        //    MonsterController mc  = monster.GetComponent<MonsterController>();
        //    mc.CellPos = pos;
        //    Managers.Object.Add(monster);
        //}

        //Managers.UI.ShowSceneUI<UI_Inven>();

        //Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;

        //gameObject.GetOrAddComponent<CursorController>();

        //GameObject player = Managers.Game.Spawn(Define.WorldObject.Player, "UnityChan");
        //Camera.main.gameObject.GetOrAddComponent<CameraController>().SetPlayer(player);
        ////Managers.Game.Spawn(Define.WorldObject.Monster, "DogKnight");
        //GameObject go = new GameObject { name = "SpawningPool" };
        //SpawningPool pool = go.GetOrAddComponent<SpawningPool>();
        //pool.SetKeepMonsterCount(5);
    }

    public override void Clear()
    {
    }
}
