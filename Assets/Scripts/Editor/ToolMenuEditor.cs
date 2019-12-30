using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mini.Core;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

public class ToolMenuEditor : Editor
{
    private static AssetsConfig config;

    [MenuItem("Assets/映射选中文件夹的Assets")]
    public static void MapSelectedDirectoryAsset()
    {

    }

    private static void MapExtensionAssets(string[] allAssets,string path)
    {

        foreach (var assetPath in allAssets)
        {
            string bundle = Path.GetDirectoryName(assetPath);

            string assetResoucesPath = assetPath.Substring(17);
            assetResoucesPath = assetResoucesPath.Substring(0, assetResoucesPath.LastIndexOf('.'));

            int startIndex = bundle.LastIndexOf('\\');
            startIndex++;
            bundle = bundle.Substring(startIndex).ToLower();
            AssetImporter.GetAtPath(assetPath).SetAssetBundleNameAndVariant(bundle, "");
            string name = Path.GetFileNameWithoutExtension(assetPath);
            GameObject obj = AssetDatabase.LoadAssetAtPath<Object>(assetPath) as GameObject;
            if (obj != null)
            {
                ManagedObject managed = obj.GetComponent<ManagedObject>();
                if (managed != null)
                {
                    managed.PrefabName = name;
                    managed.AssetBundleName = bundle;
                    PrefabUtility.SavePrefabAsset(obj);
                }
                else
                {
                    BaseView baseview = obj.GetComponent<BaseView>();
                    if (baseview != null)
                    {
                        if (baseview.GetType().Name != name)
                        {
                            Debug.LogWarningFormat("警告：路径{0}的 UI 预制体挂在的 UI 脚本类名与预制体名称不符。", assetPath);
                        }
                        baseview.ViewName = name;
                        PrefabUtility.SavePrefabAsset(obj);
                    }
                }
            }

            config.List.Add(new AssetConfigItem() { AssetBundleName = bundle, AssetsName = name,AssetPath = assetResoucesPath });
        }
    }
    private static void MapDirectoryAssets(string path)
    {
        MapExtensionAssets(Directory.GetFiles(path, "*.prefab", SearchOption.AllDirectories),path);
        MapExtensionAssets(Directory.GetFiles(path, "*.wav", SearchOption.AllDirectories), path);
        MapExtensionAssets(Directory.GetFiles(path, "*.mp3", SearchOption.AllDirectories), path);
        MapExtensionAssets(Directory.GetFiles(path, "*.asset", SearchOption.AllDirectories), path);

    }
    [MenuItem("Mini Game Tools/映射Assets")]
    public static void RemapAssetBundles()
    {
        config = ScriptableObject.CreateInstance<AssetsConfig>();
        string path = "Assets/Resources/Generated/" + typeof(AssetsConfig).Name + ".asset";
        MapDirectoryAssets("Assets/Resources/Extra Resources");
        MapDirectoryAssets("Assets/Resources/Biz");
        MapDirectoryAssets("Assets/Resources/Generated");
        AssetDatabase.CreateAsset(config, path);
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Mini Game Tools/打印Prefab路径映射")]
    public static void PrintPrefabAssetBundleMapping()
    {

        AssetsConfig config = MiniCore.GetConfig<AssetsConfig>();
        foreach (var item in config.List)
        {
            Debug.LogFormat("Prefab: {0} in AssetBundle: {1}",item.AssetsName, item.AssetBundleName);
        }
    }

    [MenuItem("Mini Game Tools/构建Asset Bundle")]
    public static void BuildAsset()
    {
        AssetsConfig ww = MiniCore.GetConfig<AssetsConfig>();
        BuildAssetBundles(null);
    }
    public static string CreateAssetBundleDirectory()
    {
        // Choose the output path according to the build target.
        string outputPath = Path.Combine(AssetManager.Utility.AssetBundlesOutputPath, AssetManager.Utility.GetPlatformName());
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        return outputPath;
    }
    public static string GetBuildTargetName(BuildTarget target)
    {
        switch (target)
        {
            case BuildTarget.Android:
                return "/mini_game_test.apk";
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return "/mini_game_test.exe";
            case BuildTarget.WebGL:
            case BuildTarget.iOS:
                return "";
            default:
                Debug.Log("Target not implemented.");
                return null;
        }
    }

    static void CopyAssetBundlesTo(string outputPath)
    {
        FileUtil.DeleteFileOrDirectory(Application.streamingAssetsPath);
        Directory.CreateDirectory(outputPath);

        string outputFolder = AssetManager.Utility.GetPlatformName();
        
        var source = Path.Combine(Path.Combine(System.Environment.CurrentDirectory, AssetManager.Utility.AssetBundlesOutputPath), outputFolder);
        if (!System.IO.Directory.Exists(source))
            Debug.Log("No assetBundle output folder, try to build the assetBundles first.");
        
        var destination = System.IO.Path.Combine(outputPath, outputFolder);
        if (System.IO.Directory.Exists(destination))
            FileUtil.DeleteFileOrDirectory(destination);

        FileUtil.CopyFileOrDirectory(source, destination);
    }

    public static void BuildAssetBundles(AssetBundleBuild[] builds)
    {
        string outputPath = "Assets/StreamingAssets";
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }
        var options = BuildAssetBundleOptions.None;

        bool shouldCheckODR = EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS;
#if UNITY_TVOS
            shouldCheckODR |= EditorUserBuildSettings.activeBuildTarget == BuildTarget.tvOS;
#endif
        if (shouldCheckODR)
        {
#if ENABLE_IOS_ON_DEMAND_RESOURCES
                if (PlayerSettings.iOS.useOnDemandResources)
                    options |= BuildAssetBundleOptions.UncompressedAssetBundle;
#endif
#if ENABLE_IOS_APP_SLICING
                options |= BuildAssetBundleOptions.UncompressedAssetBundle;
#endif
        }

        if (builds == null || builds.Length == 0)
        {
            //@TODO: use append hash... (Make sure pipeline works correctly with it.)
            BuildPipeline.BuildAssetBundles(outputPath, options, EditorUserBuildSettings.activeBuildTarget);
        }
        else
        {
            BuildPipeline.BuildAssetBundles(outputPath, builds, options, EditorUserBuildSettings.activeBuildTarget);
        }
    }
}
