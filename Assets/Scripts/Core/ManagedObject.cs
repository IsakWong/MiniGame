using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagedObject : MonoBehaviour
{
    private bool IsManaged = false;
    public string PrefabName = "Prefab";
    public string AssetBundleName = "Assets";
    public bool IsActive = true;

    protected void Awake()
    {
        if (!IsManaged)
        {
            //场景中预先存在的物体注册到对象池。
            ObjectManager.RegisterSceneObjectToPool(this);
            IsManaged = true;
        }
    }

    #region 公共接口
    public void Recycle()
    {
        this.RecycleManagerObject();
    }


    #endregion

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
