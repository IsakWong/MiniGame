using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class AssetsConfig : ScriptableObject
{
    //asset name / asset bundle
    public List<AssetConfigItem> List = new List<AssetConfigItem>();

    [NonSerialized]
    private Dictionary<string, AssetConfigItem> assetConfigDic = null;


    private void BuildDic()
    {
        assetConfigDic = new Dictionary<string, AssetConfigItem>(List.Count);
        foreach (var item in List)
        {
            assetConfigDic.Add(item.AssetsName,item);
        }
    }

    public string GetAssetPath(string name)
    {
        if (assetConfigDic == null)
            BuildDic();
        if (assetConfigDic.TryGetValue(name, out AssetConfigItem config))
            return config.AssetPath;
        LogManager.LogErrorFormat(String.Format("Error:未能找到 {0} 资源的Bundle包映射。", name));
        return "";
    }
    public string GetAssetBundle(string name)
    {
        if (assetConfigDic == null)
            BuildDic();
        if (assetConfigDic.TryGetValue(name, out AssetConfigItem config))
            return config.AssetBundleName;
        LogManager.LogErrorFormat(String.Format("Error:未能找到 {0} 资源的Bundle包映射。", name));
        return "";
    }
}

[System.Serializable]
public class AssetConfigItem
{
    public string AssetsName;
    public string AssetBundleName;
    public string AssetPath;
}