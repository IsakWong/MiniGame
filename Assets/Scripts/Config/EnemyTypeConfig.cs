using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "EnemyTypeConfig", menuName = "配置表/敌人类型表", order = 1)]
public class EnemyTypeConfig : ScriptableObject
{
    //asset name / asset bundle
    public List<EnemyInfo> EnemyTypeList = new List<EnemyInfo>(100);

    [NonSerialized]
    private Dictionary<EnemyType, EnemyInfo> dic = null;
    private void BuildDic()
    {
        dic = new Dictionary<EnemyType, EnemyInfo>(EnemyTypeList.Count);
        int i = 0;
        
        foreach (var item in EnemyTypeList)
        {
            dic.Add(item.Type, item);
        }
    }

    public EnemyInfo GetEnemyInfoByIndex(EnemyType type)
    {
        if (dic == null)
            BuildDic();
        if (dic.TryGetValue(type, out EnemyInfo info))
            return info;
        LogManager.LogErrorFormat(String.Format("Error:未能找到 {0} 资源的Bundle包映射。", info));
        return null;
    }
}



[System.Serializable]
public class EnemyInfo
{
    public int id;
    public string Name;
    public string PrefabName;
    public EnemyType Type;
}
