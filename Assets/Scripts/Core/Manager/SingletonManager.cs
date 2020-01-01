using Excel.Log;
using UnityEngine;
using UnityEditor;

public class SingletonManager<T> : MonoBehaviour where T : Component
{

    #region 单例模式
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance != null)
                return _instance;
            else
            {
                return  _instance = new GameObject("[" + typeof(T).Name + "]").AddComponent<T>();
            }
        }
    }

    public static GameObject ManagerParent;
    void Awake()
    {
        GameObject parent = GameObject.Find("[Manager]");
        LogManager.Log(string.Format("{0} Awake",GetType().Name));
        if (parent == null )
            parent = new GameObject("[Manager]");
        transform.parent = parent.transform;

        _instance = gameObject.GetComponent<T>();
    }

    public virtual void OnDispose()
    {
        DestroyManagedObjects();
    }
    protected virtual void DestroyManagedObjects()
    {

    }
    #endregion
}