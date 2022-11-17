using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using static Define;

public class PlayerController : CreatureController
{
    protected Coroutine _coAttack;
    protected bool _rangeSkill = false;
    protected override void Init()
    {
        base.Init();
    }
    protected override void UpdateAnimation()
    {
        if (State == CreatureState.Idle)
        {
            switch (_lastDir)
            {
                case MoveDir.Up:
                    _animator.Play("IDLE_BACK");
                    _spriteRenderer.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("IDLE_FRONT");
                    _spriteRenderer.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("IDLE_RIGHT");
                    _spriteRenderer.flipX = true;
                    break;
                case MoveDir.Right:
                    _animator.Play("IDLE_RIGHT");
                    _spriteRenderer.flipX = false;
                    break;
            }
        }
        else if (State == CreatureState.Moving)
        {
            switch (Dir)
            {
                case MoveDir.Up:
                    _animator.Play("WALK_BACK");
                    _spriteRenderer.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("WALK_FRONT");
                    _spriteRenderer.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("WALK_RIGHT");
                    _spriteRenderer.flipX = true;
                    break;
                case MoveDir.Right:
                    _animator.Play("WALK_RIGHT");
                    _spriteRenderer.flipX = false;
                    break;
            }
        }
        else if (State == CreatureState.Attack)
        {
            switch (_lastDir)
            {
                case MoveDir.Up:
                    _animator.Play(_rangeSkill ? "ATTACK_WEAPON_BACK":"ATTACK_BACK");
                    _spriteRenderer.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play(_rangeSkill ? "ATTACK_WEAPON_FRONT" : "ATTACK_FRONT");
                    _spriteRenderer.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play(_rangeSkill ? "ATTACK_WEAPON_RIGHT" : "ATTACK_RIGHT");
                    _spriteRenderer.flipX = true;
                    break;
                case MoveDir.Right:
                    _animator.Play(_rangeSkill ? "ATTACK_WEAPON_RIGHT" : "ATTACK_RIGHT");
                    _spriteRenderer.flipX = false;
                    break;
            }
        }
        else
        {

        }
    }
    protected override void UpdateController()
    {
        base.UpdateController();
    }
    protected override void UpdateIdle()
    {
        // 이동 상태로 갈지 확인
        if (Dir != MoveDir.None)
        {
            State = CreatureState.Moving;
            return;
        }
    }
    IEnumerator CoStartPunch()
    {
        // 피격 판정
        GameObject go = Managers.Object.Find(GetFrontCellPos());
        if(go != null)
        {
            CreatureController cc = go.GetComponent<CreatureController>();
            if (cc != null)
                cc.OnDamaged();
        }
        // 대기 시간
        _rangeSkill = false;
        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Moving;
        _coAttack = null;
    }
    IEnumerator CoStartShootArrow()
    {
        GameObject go = Managers.Resource.Instantiate("Creature/Arrow");
        ArrowController ac = go.GetComponent<ArrowController>();
        ac.Dir = _lastDir;
        ac.CellPos = CellPos;

        // 대기 시간
        _rangeSkill = true;
        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Moving;
        _coAttack = null;
    }
    public override void OnDamaged()
    {
        Debug.Log("Player HIT ! ");
    }
}
