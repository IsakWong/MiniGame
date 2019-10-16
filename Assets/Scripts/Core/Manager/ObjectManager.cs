﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class ObjectManager
{
    public static Dictionary<string,HashSet<ManagedObject>> ManagedObjectPool = new Dictionary<string, HashSet<ManagedObject>>();


    public static void RegisterSceneObjectToPool(ManagedObject managed)
    {
        HashSet<ManagedObject> unusedPool = GetObjectPool(managed.PrefabName);
    }
    
    public static void RecycleManagerObject(this MonoBehaviour behaviour)
    {
        ManagedObject managed = behaviour.GetComponent<ManagedObject>();
        GameObject result = behaviour.gameObject;
        if (managed == null)
        {
            LogManager.Log("Error:该物体无法被回收");
            return;
        }
        HashSet<ManagedObject> objectPool = GetObjectPool(managed.PrefabName);
        
        objectPool.Add(managed);
        result.BroadcastMessage("OnRecycle", SendMessageOptions.DontRequireReceiver);
        result.SetActive(false);
    }

    public static T CreateManagedObject<T>(string prefabName)
    {
        HashSet<ManagedObject> objectPool = GetObjectPool(prefabName);
        GameObject result = null;
        ManagedObject managed = null;
        if (objectPool.Count == 0)
        {
            GameObject origin = AssetManager.LoadGameObject(prefabName);
            result = GameObject.Instantiate(origin);
            managed = result.GetComponent<ManagedObject>();
            if (managed == null)
                return default(T);
        }
        else
        {
            //LogManager.Log("Log:回收物体被激活");
            managed = objectPool.First();
            result = managed.gameObject;
            objectPool.Remove(managed);
            result.SetActive(true);
        }
        result.BroadcastMessage("OnReuse");
        return result.GetComponent<T>();
    }
    public static GameObject CreateManagedObject(string prefabName)
    {
        HashSet<ManagedObject> objectPool = GetObjectPool(prefabName);
        GameObject result = null;
        ManagedObject managed = null;
        if (objectPool.Count == 0)
        {
            GameObject origin = AssetManager.LoadGameObject(prefabName);
            result = GameObject.Instantiate(origin);
            managed = result.GetComponent<ManagedObject>();
            if (managed == null)
                return null;
        }
        else
        {
            //LogManager.Log("Log:回收物体被激活");
            managed = objectPool.First();
            result = managed.gameObject;
            objectPool.Remove(managed);
            result.SetActive(true);
        }
        result.BroadcastMessage("OnReuse");
        return result;
    }
    
    private static void RegisterType(string prefabName)
    {
        HashSet<ManagedObject> unusedPool = GetObjectPool(prefabName);
    }

    private static HashSet<ManagedObject> GetObjectPool(string prefabName)
    {
        HashSet<ManagedObject> objectPool = null;
        if (!ManagedObjectPool.TryGetValue(prefabName, out objectPool))
        {
            objectPool = new HashSet<ManagedObject>();
            ManagedObjectPool.Add(prefabName, objectPool);
        }
        return objectPool;
    }
}