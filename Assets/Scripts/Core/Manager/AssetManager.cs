using System;
using System.Collections;
using System.IO;
using Mini.Core;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


public class StringObjectDictionary : SerializableDictionaryBase<string, Object> {};
public class StringBundlesDictionary : SerializableDictionaryBase<string, AssetBundle> { };
public class AssetManager : SingletonManager<AssetManager>
{
    public bool UsingAssetBundle = false;
    public StringBundlesDictionary LoadBundles = new StringBundlesDictionary();
    public StringObjectDictionary LoadAssets = new StringObjectDictionary();
    private AssetsConfig _cachedAssetConfig;

    public void Init()
    {
        _cachedAssetConfig = MiniCore.GetConfig<AssetsConfig>();
    }

    #region 接口
    /// <summary>
    /// 将Asset加载进资源表
    /// </summary>
    /// <param name="assetName"></param>
    /// <param name="callback"></param>
    public void LoadAssetAsync(string assetName, Action<bool, float> callback)
    {
        if (LoadAssets.TryGetValue(assetName, out Object obj))
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
    /// <summary>
    /// 加载Asset，如果在资源表则直接返回，不在则阻塞加载
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="assetName"></param>
    /// <returns></returns>
    public static T LoadAsset<T>(string assetName) where T : Object
    {
        return Instance._InternalLoadAsset<T>(assetName);
    }



    public AssetBundle LoadBundle(string bundleName)
    {
#if UNITY_EDITOR
        return null;
#endif
        if (LoadBundles.TryGetValue(bundleName, out AssetBundle bundle))
        {
            return bundle;
        }

        bundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, bundleName));
        LoadBundles.Add(bundleName, bundle);

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

    public static void Unload()
    {

#if !UNITY_EDITOR
        foreach (var bundle in LoadBundles)
        {
            bundle.Value.Unload(true);
        }
#endif

        Instance.LoadBundles.Clear();
        Instance.LoadAssets.Clear();
    }


    #endregion
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
        LoadAssets.Add(assetName, assetLoadRequest.asset);
        callback.Invoke(true, 1.0f);
    }
    #endregion

    #region 异步资源 AssetBundle 加载

    public void LoadAssetAtAssetBundleAsync(string assetName, string bundleName, Action<bool, float> callback)
    {
        if (LoadBundles.TryGetValue(bundleName, out AssetBundle bundle))
        {
            StartCoroutine(_LoadAssetAtAssetBundleAsync(bundle, assetName, callback));
            return;
        }
        else
        {
            string bundlePath = Path.Combine(Application.streamingAssetsPath, bundleName);
            StartCoroutine(_LoadAssetBundleAsync(bundlePath, obj =>
            {
                bundle = LoadBundles[bundleName];
                StartCoroutine(_LoadAssetAtAssetBundleAsync(bundle, assetName, callback));
            }));

        }
    }

    IEnumerator _LoadAssetBundleAsync(string bundlePath, Action<bool> callback)
    {
        var bundleLoadRequest = AssetBundle.LoadFromFileAsync(bundlePath);
        yield return bundleLoadRequest;
        LoadBundles.Add(bundleLoadRequest.assetBundle.name, bundleLoadRequest.assetBundle);
        callback.Invoke(true);
    }

    IEnumerator _LoadAssetAtAssetBundleAsync(AssetBundle bundle, string assetName, Action<bool, float> callback)
    {
        var assetLoadRequest = bundle.LoadAssetAsync<Object>(assetName);
        callback.Invoke(false, assetLoadRequest.progress);
        yield return assetLoadRequest;
        LoadAssets.Add(assetName, assetLoadRequest.asset);
        callback.Invoke(true, 1.0f);
    }

    #endregion
    private T _InternalLoadAsset<T>(string assetName) where T : Object
    {
        string bundleName;

#if UNITY_EDITOR
        bundleName = _cachedAssetConfig.GetAssetBundle(assetName);
        if (LoadAssets.TryGetValue(assetName, out Object obj1))
        {
            return (T)obj1;
        }
        #region Asset Cache
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
                    config.WorldAssetListDic.Add(game.WorldIndex, value);
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
        LoadAssets.Add(assetName, target);
        return (T)target;
        #endregion
#endif

        if (LoadAssets.TryGetValue(assetName, out Object obj))
        {
            return (T)obj;
        }

        if (UsingAssetBundle)
        {
            bundleName = _cachedAssetConfig.GetAssetBundle(assetName);

            if (!LoadBundles.TryGetValue(bundleName, out AssetBundle bundle))
                bundle = LoadBundle(bundleName);

            obj = bundle.LoadAsset<T>(assetName);
            LoadAssets.Add(assetName, obj);
            return (T)obj;
        }
        else
        {
            string path = _cachedAssetConfig.GetAssetPath(assetName);
            T t = Resources.Load<T>(path);
            LoadAssets.Add(assetName,t);
            return t;
        }
    
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
