using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEditor;
using UnityEngine;
public static class ObjectManagerExt
{
    public static void RecycleManagerObject(this MonoBehaviour behaviour)
    {
        ManagedObject managed = behaviour.GetComponent<ManagedObject>();
        GameObject result = behaviour.gameObject;
        if (managed == null)
        {
            LogManager.Log("Error:该物体无法被回收");
            return;
        }
        HashSet<ManagedObject> objectPool = ObjectManager.Instance.GetObjectPool(managed.PrefabName);

        objectPool.Add(managed);
        result.BroadcastMessage("OnRecycle", SendMessageOptions.DontRequireReceiver);
        result.SetActive(false);
    }
}

public class StringObjectPoolDictionary : SerializableDictionaryBase<string, HashSet<ManagedObject>>
{
};
public class ObjectManager : SingletonManager<ObjectManager>
{
    public StringObjectPoolDictionary ManagedObjectPool = new StringObjectPoolDictionary();

 
    public void RegisterSceneObjectToPool(ManagedObject managed)
    {
        HashSet<ManagedObject> unusedPool = GetObjectPool(managed.PrefabName);
    }
    


    public static T CreateManagedObject<T>(string prefabName)
    {
        HashSet<ManagedObject> objectPool = Instance.GetObjectPool(prefabName);
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
        HashSet<ManagedObject> objectPool = Instance.GetObjectPool(prefabName);
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
    
    private void RegisterType(string prefabName)
    {
        HashSet<ManagedObject> unusedPool = GetObjectPool(prefabName);
    }

    public HashSet<ManagedObject> GetObjectPool(string prefabName)
    {
        HashSet<ManagedObject> objectPool = null;
        if (!ManagedObjectPool.TryGetValue(prefabName, out objectPool))
        {
            objectPool = new HashSet<ManagedObject>();
            ManagedObjectPool.Add(prefabName, objectPool);
        }
        return objectPool;
    }

    protected override void DestroyManagedObjects()
    {
        LogManager.Log("Clearing Objects....", Color.blue);
        foreach (var pool in Instance.ManagedObjectPool.Values)
        {
            foreach (var gameobject in pool)
            {
                GameObject.Destroy(gameobject);
            }
        }
        LogManager.Log("Clear Objects Succeeded....", Color.blue);
    }
}


