using UnityEngine;
using UnityEditor;

public class SingletonManager<T> : MonoBehaviour where T : Component
{

    #region 单例模式
    private static T _instance;
    public static T Instance
    {
        get { return _instance != null ? _instance : _instance = new GameObject("[" + typeof(T).Name + "]").AddComponent<T>(); }
    }

    public static GameObject ManagerParent;
    void Awake()
    {
        GameObject parent = GameObject.Find("[Manager]");
        if (parent == null )
            parent = new GameObject("[Manager]");
        transform.parent = parent.transform;

        _instance = gameObject.GetComponent<T>();
    }
    #endregion
}