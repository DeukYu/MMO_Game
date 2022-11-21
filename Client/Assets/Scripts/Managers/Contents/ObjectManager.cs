using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class ObjectManager
{
    public MyPlayerController MyPlayer { get; set; }
    Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();

    public void Add(ObjectInfo info, bool myPlayer = false)
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
