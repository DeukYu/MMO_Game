using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class ObjectManager
{
    public MyPlayerController MyPlayer { get; set; }
    Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();

    public static GameObjectType GetObjectTypeById(int id)
    {
        int type = (id >> 24) & 0x7F;
        return (GameObjectType)type;
    }
    public void Add(ObjectInfo info, bool myPlayer = false)
    {
        GameObjectType objectType = GetObjectTypeById(info.ObjectId);
        if(objectType == GameObjectType.Player)
        {
            if (myPlayer)
            {
                GameObject go = Managers.Resource.Instantiate("Creature/MyPlayer");
                go.name = info.Name;
                _objects.Add(info.ObjectId, go);

                MyPlayer = go.GetComponent<MyPlayerController>();
                MyPlayer.Id = info.ObjectId;
                MyPlayer.PosInfo = info.PosInfo;
                MyPlayer.SyncPos();
            }
            else
            {
                GameObject go = Managers.Resource.Instantiate("Creature/Player");
                go.name = info.Name;
                _objects.Add(info.ObjectId, go);

                PlayerController pc = go.GetComponent<PlayerController>();
                pc.Id = info.ObjectId;
                pc.PosInfo = info.PosInfo;
                pc.SyncPos();
            }
        }
        else if(objectType == GameObjectType.Monster)
        {
            GameObject go = Managers.Resource.Instantiate("Creature/Monster");
            if (go == null)
                return;

            go.name = "Monster";
            _objects.Add(info.ObjectId, go);
        }
        else if(objectType == GameObjectType.Projectile)
        {
            GameObject go = Managers.Resource.Instantiate("Creature/Arrow");
            if (go == null)
                return;
            go.name = "Arrow";
            _objects.Add(info.ObjectId, go);

            ArrowController ac = go.GetComponent<ArrowController>();
            if (ac == null)
                return;
            ac.Dir = info.PosInfo.MoveDir;
            ac.CellPos = new Vector3Int(info.PosInfo.PosX, info.PosInfo.PosY, 0);
            ac.SyncPos();
        }
    }
    public void Remove(int id)
    {
        GameObject go = FindById(id);
        if (go == null)
            return;
        Managers.Resource.Destroy(go);
        _objects.Remove(id);
    }
    public void RemoveMyPlayer()
    {
        if (MyPlayer == null)
            return;

        Remove(MyPlayer.Id);
        MyPlayer = null;
    }
    public GameObject FindById(int id)
    {
        GameObject go = null;
        _objects.TryGetValue(id, out go);
        return go;
    }
    public GameObject Find(Vector3Int cellPos)
    {
        foreach (GameObject obj in _objects.Values)
        {
            CreatureController cc = obj.GetComponent<CreatureController>();
            if (cc == null)
                continue;
            if (cc.CellPos == cellPos)
                return obj;
        }
        return null;
    }
    public GameObject Find(Func<GameObject, bool> condition)
    {
        foreach (GameObject obj in _objects.Values)
        {
            CreatureController cc = obj.GetComponent<CreatureController>();
            if (cc == null)
                continue;

            if (condition.Invoke(obj))
                return obj;
        }

        return null;
    }
    public void Clear()
    {
        foreach (GameObject obj in _objects.Values)
            Managers.Resource.Destroy(obj);
        _objects.Clear();
    }
}
