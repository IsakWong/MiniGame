using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

[System.Serializable]
public class ViewDic : SerializableDictionaryBase<string, BaseView> { }
[System.Serializable]
public class CanavsDic : SerializableDictionaryBase<string, UICanvas> { }

public class ViewManager : SingletonManager<ViewManager>
{


    public UICanvas CurrentMainCanvas;

    public bool CreatingCanvas = false;


    [SerializeField]
    public ViewDic Views = new ViewDic();
    [SerializeField]
    public CanavsDic CanvasDictionary = new CanavsDic();



    #region 公共接口

    public static T CreateCanvas<T>() where T : UICanvas
    {
        Instance.CreatingCanvas = true;
        T t = GameObject.Instantiate(AssetManager.LoadGameObject(typeof(T).Name).GetComponent<T>());
        Instance.CanvasDictionary.Add(typeof(T).Name, t);
        if (typeof(T) == typeof(MainCanvas))
            Instance.CurrentMainCanvas = t;
        Instance.CreatingCanvas = false;
        return t;
    }

    public static void Clear()
    {

    }

    public static UICanvas GetCanvas(string name)
    {
        if (Instance.CanvasDictionary.TryGetValue(name, out UICanvas canvas))
        {
            return canvas;
        }
        return null;
    }
    public static T GetCanvas<T>() where T : UICanvas
    {
        if (Instance.CanvasDictionary.TryGetValue(typeof(T).Name, out UICanvas canvas))
        {
            return (T)canvas;
        }
        return null;
    }
    public static void CombineCanvas(UICanvas newCanvas)
    {
        UICanvas old = ViewManager.GetCanvas(newCanvas.GetType().Name);
        if (old != null)
        {
            for (int i = 0; i < old.transform.childCount; ++i)
            {
                old.transform.GetChild(i).transform.SetParent(newCanvas.transform, true);
            }

            Instance.CanvasDictionary[old.GetType().Name] = newCanvas;
            GameObject.Destroy(old.gameObject, 0.1f);
        }
        if (newCanvas.GetType() == typeof(MainCanvas))
            Instance.CurrentMainCanvas = newCanvas;
    }
    public static bool RemoveView<T>(string name = null) where T : BaseView
    {
        if (name == null) name = typeof(T).Name;
        if (!Instance.Views.TryGetValue(name, out var b))
            return false;
        Instance.Views.Remove(name);
        return true;
    }
    public static T GetView<T>(bool create = true) where T : BaseView
    {
        return GetView<T, MainCanvas>(create);
    }

    public static T GetView<T, T2>(bool create = true)
        where T : BaseView
        where T2 : UICanvas
    {
        return GetView<T, T2>(typeof(T).Name, create); ;
    }

    public static T GetView<T, T2>(string name, bool create = true)
        where T : BaseView
        where T2 : UICanvas
    {
        if (Instance.Views.TryGetValue(name, out var b))
            return b.GetComponent<T>();
        if (create)
        {
            GameObject origin = AssetManager.LoadGameObject(name);
            if (!Instance.CanvasDictionary.TryGetValue(typeof(T2).Name, out UICanvas canvas))
            {
                canvas = CreateCanvas<T2>();
            }
            T view = GameObject.Instantiate(origin, canvas.transform).GetComponent<T>();
            return view;
        }
        return null;
    }


    #endregion
    protected override void DestroyManagedObjects()
    {
        LogManager.Log("Clearing Views....", Color.blue);
        {

            List<BaseView> ToDestroyList = Instance.Views.Values.ToList();
            for (int i = 0; i < ToDestroyList.Count; ++i)
            {
                GameObject.DestroyImmediate(ToDestroyList[i]);
                ToDestroyList[i] = null;
            }
        }
        LogManager.Log("Clear Views Succeeded....", Color.blue);


        LogManager.Log("Clearing Canvas....", Color.blue);
        {

            List<UICanvas> ToDestroyList = Instance.CanvasDictionary.Values.ToList();
            for (int i = 0; i < ToDestroyList.Count; ++i)
            {
                GameObject.DestroyImmediate(ToDestroyList[i]);
                ToDestroyList[i] = null;
            }
        }
        LogManager.Log("Clear Canvas Succeeded....", Color.blue);
    }

}
