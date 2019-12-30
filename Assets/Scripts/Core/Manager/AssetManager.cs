using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Mini.Core;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class AssetManager : SingletonManager<AssetManager>
{
    public bool UsingAssetBundle = false;
    public Dictionary<string, AssetBundle> bundles = new Dictionary<string, AssetBundle>();
    public Dictionary<string, Object> loadedAssets = new Dictionary<string, Object>();
    private AssetsConfig _cachedAssetConfig;

    public void Init()
    {
        _cachedAssetConfig = MiniCore.GetConfig<AssetsConfig>();

    }

    #region 异步资源 Resource 加载
    public void LoadAssetAtResourceAsync(string assetName, string assetPath, Action<bool, float> callback)
    {
        StartCoroutine(_LoadAssetAtResourceAsync(assetName, assetPath, callback));
    }
    IEnumerator _LoadAssetAtResourceAsync(string assetName, string assetPath, Action<bool, float> callback)
    {
        var assetLoadRequest = Resources.LoadAsync(assetPath);
        callback.Invoke(false, assetLoadRequest.progress);
        yield return assetLoadRequest;
        loadedAssets.Add(assetName, assetLoadRequest.asset);
        callback.Invoke(true, 1.0f);
    }
    #endregion

    #region 异步资源 AssetBundle 加载

    public void LoadAssetAtAssetBundleAsync(string assetName, string bundleName, Action<bool, float> callback)
    {
        if (bundles.TryGetValue(bundleName, out AssetBundle bundle))
        {
            StartCoroutine(_LoadAssetAtAssetBundleAsync(bundle, assetName, callback));
            return;
        }
        else
        {
            string bundlePath = Path.Combine(Application.streamingAssetsPath, bundleName);
            StartCoroutine(_LoadAssetBundleAsync(bundlePath, obj =>
            {
                bundle = bundles[bundleName];
                StartCoroutine(_LoadAssetAtAssetBundleAsync(bundle, assetName, callback));
            }));

        }
    }

    IEnumerator _LoadAssetBundleAsync(string bundlePath, Action<bool> callback)
    {
        var bundleLoadRequest = AssetBundle.LoadFromFileAsync(bundlePath);
        yield return bundleLoadRequest;
        bundles.Add(bundleLoadRequest.assetBundle.name, bundleLoadRequest.assetBundle);
        callback.Invoke(true);
    }

    IEnumerator _LoadAssetAtAssetBundleAsync(AssetBundle bundle, string assetName, Action<bool, float> callback)
    {
        var assetLoadRequest = bundle.LoadAssetAsync<Object>(assetName);
        callback.Invoke(false, assetLoadRequest.progress);
        yield return assetLoadRequest;
        loadedAssets.Add(assetName, assetLoadRequest.asset);
        callback.Invoke(true, 1.0f);
    }

    #endregion



    public void LoadAssetAsync(string assetName, Action<bool, float> callback)
    {
        if (loadedAssets.TryGetValue(assetName, out Object obj))
        {
            callback.Invoke(true, 1.0f);
            return;
        }
        if (UsingAssetBundle)
        {
            string bundleName = _cachedAssetConfig.GetAssetBundle(assetName);
            LoadAssetAtAssetBundleAsync(assetName, bundleName, callback);
        }
        else
        {
            string path = _cachedAssetConfig.GetAssetPath(assetName);
            LoadAssetAtResourceAsync(assetName, path, callback);
        }

    }
    public static GameObject LoadGameObject(string assetName)
    {
        return LoadAsset<GameObject>(assetName);
    }
    public static T LoadAsset<T>(string assetName) where T : Object
    {
        return Instance._InternalLoadAsset<T>(assetName);
    }
    private T _InternalLoadAsset<T>(string assetName) where T : Object
    {
        string bundleName;
#if UNITY_EDITOR
        bundleName = _cachedAssetConfig.GetAssetBundle(assetName);
        if (loadedAssets.TryGetValue(assetName, out Object obj1))
        {
            return (T)obj1;
        }

        var game = MiniCore.Get<GameController>(false);
        if (game != null)
        {
            if (MiniCore.CoreBehaviour.SaveCache)
            {
                string path = "Assets/Resources/Generated/WorldCacheAssetConfig.asset";
                WorldCacheAssetConfig config = AssetDatabase.LoadAssetAtPath<WorldCacheAssetConfig>(path);
                if (!config.WorldAssetListDic.TryGetValue(game.WorldIndex, out WorldAssetList value))
                {
                    value = new WorldAssetList();
                    value.WorldIndex = game.WorldIndex;
                    config.WorldAssetListDic.Add(game.WorldIndex,value);
                }
                if (!value.AssetListSet.Contains(assetName))
                {
                    if (!game.IsLoading)
                        Debug.LogWarningFormat("资源:{0}缓存未命中", assetName);
                    value.AssetListSet.Add(assetName);
                }

                EditorUtility.SetDirty(config);
            }
          
        }
        else
        {

        }

        string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(bundleName, assetName);
        if (assetPaths.Length == 0)
        {
            LogManager.Log(
                String.Format("{0} Error:未在Prefab表中找到 {1} 资源的映射位置。请确定 {2} 的Prefab是否存在，并使用Mini Game Toll/生成Prefab映射修复",
                    typeof(AssetManager).Name, assetName, assetName)
                );
            return null;
        }
        Object target = AssetDatabase.LoadAssetAtPath<Object>(assetPaths[0]);
        loadedAssets.Add(assetName, target);
        return (T)target;
#endif

        if (loadedAssets.TryGetValue(assetName, out Object obj))
        {
            return (T)obj;
        }

        if (UsingAssetBundle)
        {
            bundleName = _cachedAssetConfig.GetAssetBundle(assetName);

            if (!bundles.TryGetValue(bundleName, out AssetBundle bundle))
                bundle = LoadBundle(bundleName);

            obj = bundle.LoadAsset<T>(assetName);
            loadedAssets.Add(assetName, obj);
            return (T)obj;
        }
        else
        {
            string path = _cachedAssetConfig.GetAssetPath(assetName);
            T t = Resources.Load<T>(path);
            loadedAssets.Add(assetName,t);
            return t;
        }
    
    }

    public static void Unload()
    {

#if !UNITY_EDITOR
        foreach (var bundle in bundles)
        {
            bundle.Value.Unload(true);
        }
#endif

        Instance.bundles.Clear();
        Instance.loadedAssets.Clear();
    }


    public AssetBundle LoadBundle(string bundleName)
    {
#if UNITY_EDITOR
        return null;
#endif
        if (bundles.TryGetValue(bundleName, out AssetBundle bundle))
        {
            return bundle;
        }

        bundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, bundleName));
        bundles.Add(bundleName, bundle);

        // TODO AssetBundle依赖
        /*AssetBundleManifest manifest = bundle.InternalLoadAsset<AssetBundleManifest>("AssetBundleManifest");
        if (manifest != null)
        {
            string[] dependencies = manifest.GetAllDependencies(bundleName);

            foreach (var dependency in dependencies)
            {
                LoadBundle(dependency);
            }
        }*/
        return bundle;
    }

    public class Utility
    {
        public const string AssetBundlesOutputPath = "AssetBundles";

        public static string GetPlatformName()
        {
#if UNITY_EDITOR
            return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
#else
            return GetPlatformForAssetBundles(Application.platform);
#endif
        }

#if UNITY_EDITOR
        private static string GetPlatformForAssetBundles(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.Android:
                    return "Android";
                case BuildTarget.iOS:
                    return "iOS";
                case BuildTarget.WebGL:
                    return "WebGL";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "Windows";
                default:
                    return null;
            }
        }
#endif

        private static string GetPlatformForAssetBundles(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                case RuntimePlatform.WebGLPlayer:
                    return "WebGL";
                case RuntimePlatform.WindowsPlayer:
                    return "Windows";
                case RuntimePlatform.OSXPlayer:
                    return "OSX";
                default:
                    return null;
            }
        }
    }
}
