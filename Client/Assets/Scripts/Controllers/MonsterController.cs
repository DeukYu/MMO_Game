using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    Coroutine _coAttack;

    [SerializeField]
    bool _rangedSkill = false;
    protected override void Init()
    {
        base.Init();

        State = CreatureState.Idle;
        Dir = MoveDir.Down;
    }
    protected override void UpdateIdle()
    {
        base.UpdateIdle();
    }
    protected override void UpdateController()
    {
        base.UpdateController();
    }
    public override void OnDamaged()
    {
        //Managers.Object.Remove(Id);
        //Managers.Resource.Destroy(gameObject);
    }
    IEnumerator CoStartPunch()
    {
        // 피격 판정
        GameObject go = Managers.Object.FindCreature(GetFrontCellPos());
        if (go != null)
        {
            CreatureController cc = go.GetComponent<CreatureController>();
            if (cc != null)
                cc.OnDamaged();
        }
        // 대기 시간  
        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Idle;
        _coAttack = null;
    }
    IEnumerator CoStartShootArrow()
    {
        GameObject go = Managers.Resource.Instantiate("Creature/Arrow");
        ArrowController ac = go.GetComponent<ArrowController>();
        ac.Dir = Dir;
        ac.CellPos = CellPos;

        // 대기 시간
        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Idle;
        _coAttack = null;
    }
}
