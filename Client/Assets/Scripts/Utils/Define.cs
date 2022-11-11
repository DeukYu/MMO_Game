using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define : MonoBehaviour
{
    public enum CreatureState
    {
        Idle,
        Moving,
        Attack,
        Skill,
        Dead,
    }
    public enum MoveDir
    {
        None,
        Up,
        Down,
        Left,
        Right,
    }
    public enum Scene
    {
        Unknown,
        Login,
        Lobby,
        Game,
    }
    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount,
    }
    public enum UIEvent
    {
        Click,
        Drag,
    }
}
