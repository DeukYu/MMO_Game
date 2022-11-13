using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    Coroutine _coPatrol;
    Coroutine _coSearch;

    [SerializeField]
    Vector3Int _destCellPos;

    [SerializeField]
    GameObject _target;

    [SerializeField]
    float _searchRange = 5.0f;
    public override CreatureState State
    {
        get { return _state; }
        set
        {
            if (_state == value)
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
        State = CreatureState.Idle;
        Dir = MoveDir.None;

        _speed = 3.0f;
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
        }

        List<Vector3Int> path = Managers.Map.FindPath(CellPos, destPos, ignoreDestCollision: true);
        if(path.Count < 2 || (_target != null &&path.Count > 10))
        {
            _target = null;
            State = CreatureState.Idle;
            return;
        }

        Vector3Int nextPos = path[1];

        Vector3Int moveCellDir = nextPos - CellPos;
        // TODO
        if (moveCellDir.x > 0)
            Dir = MoveDir.Right;
        else if (moveCellDir.x < 0)
            Dir = MoveDir.Left;
        else if (moveCellDir.y > 0)
            Dir = MoveDir.Up;
        else if (moveCellDir.y < 0)
            Dir = MoveDir.Down;
        else
            Dir = MoveDir.None;

        //Vector3Int destPos = CellPos;

        //switch (_dir)
        //{
        //    case MoveDir.Up:
        //        destPos += Vector3Int.up;
        //        break;
        //    case MoveDir.Down:
        //        destPos += Vector3Int.down;
        //        break;
        //    case MoveDir.Left:
        //        destPos += Vector3Int.left;
        //        break;
        //    case MoveDir.Right:
        //        destPos += Vector3Int.right;
        //        break;
        //}
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
        GameObject effect = Managers.Resource.Instantiate("Effect/DieEffect");
        effect.transform.position = transform.position;
        effect.GetComponent<Animator>().Play("START");
        GameObject.Destroy(effect, 0.5f);

        Managers.Object.Remve(gameObject);
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
}
