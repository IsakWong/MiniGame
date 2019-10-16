using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetPost : AssetPostprocessor
{
    static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        foreach (var assetName in importedAssets)
        {
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(assetName);
            if (obj != null)
            {
                string bundle = Path.GetDirectoryName(assetName);
                //bundle = bundle.Substring(bundle.LastIndexOf(Path.DirectorySeparatorChar));
                //AssetImporter.GetAtPath(assetName).SetAssetBundleNameAndVariant(bundle, "");
                //Debug.Log("Managed Object Prefab Asset Renamed!");
            }
        }
        foreach (var assetName in movedAssets)
        {
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(assetName);
            if (obj != null)
            {
                string bundle = Path.GetDirectoryName(assetName);
                //bundle = bundle.Substring(bundle.LastIndexOf(Path.DirectorySeparatorChar));
                //AssetImporter.GetAtPath(assetName).SetAssetBundleNameAndVariant(bundle, "");
                //Debug.Log("Managed Object Prefab Asset Renamed!");
            }
        }
    }
}

