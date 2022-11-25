using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    protected override void Init()
    {
        base.Init();
    }
    protected override void UpdateIdle()
    {
        base.UpdateIdle();
    }
    public override void OnDamaged()
    {
        //Managers.Object.Remove(Id);
        //Managers.Resource.Destroy(gameObject);
    }
    public override void UseAttack()
    {
        State = CreatureState.Attack;
    }
}
