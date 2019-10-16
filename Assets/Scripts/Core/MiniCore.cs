using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Object = UnityEngine.Object;

public static class MiniCore
{
    private static float _TimeScale = 1.0f;

    public static float TimeScale
    {
        get { return _TimeScale; }
        set
        {
            var game = Get<GameController>();
        }
    }
    
    //唯一的多场景存在的物体
    public static MiniCoreBehaviour CoreBehaviour = null;

    public static LinkedList<Command> CoreCommands = new LinkedList<Command>();

    public static Dictionary<string, ScriptableObject> LoadedConfig = new Dictionary<string, ScriptableObject>();

    public static Dictionary<string, BaseController> Controllers = new Dictionary<string, BaseController>();


    public static void PlaySound(string soundName)
    {

        GameObject obj = ObjectManager.CreateManagedObject("Sound");
        obj.GetComponent<SoundObject>().SetData(AssetManager.LoadAsset<AudioClip>(soundName));
    }
    public static void ChangeBgm(string newBgm, bool forceChange = false)
    {
        CoreBehaviour.ChangeBGM(newBgm, forceChange);
    }    public static void MuteBgm()
    {
        CoreBehaviour.GlobalBgmAudio.Pause();
    }
    public static void ContinueBgm()
    {
        CoreBehaviour.GlobalBgmAudio.UnPause();
    }

    public static void CallLoadAsset(string assetName, Func<bool, float, bool> callback)
    {
        LoadAssetCommand cmd = new LoadAssetCommand(assetName, callback);
        MiniCore.Call(cmd);
    }
    public static void Call(Command cmd)
    {
        CoreCommands.AddLast(cmd);
    }

    public static bool Empty()
    {
        return CoreCommands.Count == 0;
    }
    public static void Execute()
    {
        if (!Empty())
        {
            var first = CoreCommands.First.Value;
            if (first.IsFinished)
            {
                CoreCommands.RemoveFirst();
            }
            else
            {
                if (first.IsExcuting)
                {
                    first.Executing();
                    return;
                }
                else
                {
                    first.Execute();
                }
            }


        }
    }
    public static T Get<T>(this MonoBehaviour behaviour) where T : BaseController
    {
        if (Controllers.TryGetValue(typeof(T).Name, out BaseController controller))
        {
            return (T)controller;
        }
        T t = Activator.CreateInstance<T>();
        Controllers.Add(typeof(T).Name, t);
        t.Init();
        return t;
    }
    public static T Get<T>(bool create = true) where T : BaseController
    {
        if (Controllers.TryGetValue(typeof(T).Name, out BaseController controller))
        {
            return (T)controller;
        }

        if (create)
        {
            T t = Activator.CreateInstance<T>();
            Controllers.Add(typeof(T).Name, t);
            t.Init(); return t;
        }

        return null;
    }
    public static T GetConfig<T>(string configName = null) where T : ScriptableObject
    {
        string path;
        if (configName == null)
        {
            configName = typeof(T).Name;
        }
        if (typeof(T) == typeof(AssetsConfig))
        {
            if (!AssetManager.loadedAssets.TryGetValue(configName, out Object t))
            {
                path = "Generated/" + configName;
                t = Resources.Load<T>(path);
                AssetManager.loadedAssets.Add(configName, t);
            }
            return (T)t;
        }
        return AssetManager.LoadAsset<T>(configName);
    }
    public static T GetConfig<T>(this MonoBehaviour behaviour, string configName = null) where T : ScriptableObject
    {
        return GetConfig<T>(configName);
    }
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
    }
}
