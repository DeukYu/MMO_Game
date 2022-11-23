using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    Coroutine _coPatrol;
    Coroutine _coSearch;
    Coroutine _coAttack;

    [SerializeField]
    Vector3Int _destCellPos;

    [SerializeField]
    GameObject _target;

    [SerializeField]
    float _searchRange = 10.0f;

    [SerializeField]
    float _attackRange = 1.0f;

    [SerializeField]
    bool _rangedSkill = false;
    public override CreatureState State
    {
        get { return PosInfo.State; }
        set
        {
            if (State == value)
                return;

            base.State = value;
            if (_coPatrol != null)
            {
                StopCoroutine(_coPatrol);
                _coPatrol = null;
            }

            if (_coSearch != null)
            {
                StopCoroutine(_coSearch);
                _coSearch = null;
            }
        }
    }
    protected override void Init()
    {
        base.Init();
        AddHpBar();

        State = CreatureState.Idle;
        Dir = MoveDir.Down;

        _rangedSkill = true;//(Random.Range(0, 2) == 0 ? true : false);

        if (_rangedSkill)
            _attackRange = 10.0f;
        else
            _attackRange = 1.0f;
    }
    protected override void UpdateIdle()
    {
        base.UpdateIdle();

        if (_coPatrol == null)
        {
            _coPatrol = StartCoroutine("CoPatrol");
        }
        if (_coSearch == null)
        {
            _coSearch = StartCoroutine("CoSearch");
        }
    }
    protected override void MoveToNextPos()
    {
        Vector3Int destPos = _destCellPos;
        if(_target != null)
        {
            destPos = _target.GetComponent<CreatureController>().CellPos;

            Vector3Int dir = destPos - CellPos;
            if(dir.magnitude <= _attackRange && (dir.x == 0 || dir.y == 0))
            {
                Dir = GetDirFromVec(dir);
                State = CreatureState.Attack;

                if (_rangedSkill)
                    _coAttack = StartCoroutine("CoStartShootArrow");
                else
                    _coAttack = StartCoroutine("CoStartPunch");
                return;
            }
        }

        List<Vector3Int> path = Managers.Map.FindPath(CellPos, destPos, ignoreDestCollision: true);
        if(path.Count < 2 || (_target != null &&path.Count > 20))
        {
            _target = null;
            State = CreatureState.Idle;
            return;
        }

        Vector3Int nextPos = path[1];

        Vector3Int moveCellDir = nextPos - CellPos;
        
        Dir = GetDirFromVec(moveCellDir);

        if (Managers.Map.CanGo(destPos) && Managers.Object.Find(nextPos) == null)
        {
            CellPos = nextPos;
        }
        else
        {
            State = CreatureState.Idle;
        }
    }
    protected override void UpdateController()
    {
        base.UpdateController();
    }
    public override void OnDamaged()
    {
        Managers.Object.Remove(Id);
        Managers.Resource.Destroy(gameObject);
    }
    IEnumerator CoPatrol()
    {
        int waitSeconds = Random.Range(1, 4);
        yield return new WaitForSeconds(waitSeconds);

        for (int i = 0; i < 10; i++)
        {
            int xRange = Random.Range(-5, 6);
            int yRange = Random.Range(-5, 6);
            Vector3Int randPos = CellPos + new Vector3Int(xRange, yRange, 0);

            if (Managers.Map.CanGo(randPos) && Managers.Object.Find(randPos) == null)
            {
                _destCellPos = randPos;
                State = CreatureState.Moving;
                yield break;
            }
        }

        State = CreatureState.Idle;
    }
    IEnumerator CoSearch()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            if (_target != null)
                continue;

            _target = Managers.Object.Find((go) =>
            {
                PlayerController pc = go.GetComponent<PlayerController>();
                if (pc == null)
                    return false;

                Vector3Int dir = (pc.CellPos - CellPos);
                if (dir.magnitude > _searchRange)
                    return false;
                return true;
            });
        }
    }
    IEnumerator CoStartPunch()
    {
        // 피격 판정
        GameObject go = Managers.Object.Find(GetFrontCellPos());
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
