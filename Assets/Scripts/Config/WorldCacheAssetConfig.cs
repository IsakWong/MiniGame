using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName = "WorldCacheAssetConfig", menuName = "配置表/世界资源缓存", order = 1)]
public class WorldCacheAssetConfig : ScriptableObject
{
    [SerializeField]
    public List<WorldAssetList> SerializedList;

    [NonSerialized]
    //asset name / asset bundle
    private Dictionary<int, WorldAssetList> _worldAssetListDic;

    public Dictionary<int, WorldAssetList> WorldAssetListDic
    {
        get
        {
            if (_worldAssetListDic == null)
            {
                _worldAssetListDic = new Dictionary<int, WorldAssetList>(SerializedList.Count);
                foreach (var world in SerializedList)
                {
                    _worldAssetListDic.Add(world.WorldIndex, world);
                }
            }
            return _worldAssetListDic;
        }
    }

    public void Save()
    {
        if (_worldAssetListDic == null)
        {

        }
        else
        {
            SerializedList.Clear();
            foreach (var asset in _worldAssetListDic.Values)
            {
                asset.Save();
                SerializedList.Add(asset);
            }
        }

    }
    public HashSet<string> GetWorldCacheAssetsByIndex(int index)
    {
        if(WorldAssetListDic.ContainsKey(index))
            return WorldAssetListDic[index].AssetListSet;
        return null;
    }

}

[System.Serializable]
public class WorldAssetList
{
    [NonSerialized] private HashSet<string> set;

    public HashSet<string> AssetListSet
    {
        get
        {
            if (set == null)
            {
                set = new HashSet<string>();
                foreach (var world in SerializedList)
                {
                    set.Add(world);
                }
            }
            return set;
        }
    }

    public void Save()
    {
        if (set == null)
        {

        }
        else
        {
            SerializedList.Clear();
            foreach (var asset in set)
            {
                SerializedList.Add(asset);
            }
        }
    }

    [SerializeField]
    public int WorldIndex;

    [SerializeField]
    public List<string> SerializedList = new List<string>();
}