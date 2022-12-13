using UnityEngine;

public class GameScene : BaseScene
{
    UI_GameScene _sceneUI;
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Game;

        Managers.Map.LoadMap(1);

        _sceneUI  = Managers.UI.ShowSceneUI<UI_GameScene>();
    }

    public override void Clear()
    {
    }
}
