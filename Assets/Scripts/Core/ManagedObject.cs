using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagedObject : MonoBehaviour
{
    private bool IsManaged = false;

    public static Dictionary<string, GameObject> CategoryParent = new Dictionary<string, GameObject>();
    public string PrefabName = "Prefab";
    public string AssetBundleName = "Assets";
    public bool IsActive = true;

    protected void Awake()
    {
        if (!IsManaged)
        {
            //场景中预先存在的物体注册到对象池。
            ObjectManager.Instance.RegisterSceneObjectToPool(this);
            IsManaged = true;
        }

        string category = GetCategory();
        if (category != "")
        {
            if (!CategoryParent.TryGetValue(category, out GameObject target))
            {
                target = new GameObject(category);
                CategoryParent.Add(category, target);
            }
            transform.parent = target.transform;
        }
    }

    #region 公共接口
    public void Recycle()
    {
        this.RecycleManagerObject();
    }


    #endregion


    protected virtual string GetCategory()
    {
        return "";
    }
    protected virtual void OnRecycle()
    {
    }
    protected virtual void OnReuse()
    {
    }

    protected void Start()
    {

    }

    protected void FixedUpdate()
    {
        
    }
}
