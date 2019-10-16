using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager
{
    public static Dictionary<string, BaseView> Views = new Dictionary<string, BaseView>();
    public static Dictionary<string, UICanvas> CanvasDic = new Dictionary<string, UICanvas>();

    public static UICanvas CurrentMainCanvas;

    public static bool CreatingCanvas = false;

    public static void Init()
    {

    }
    public static T CreateCanvas<T>() where T : UICanvas
    {
        CreatingCanvas = true;
        T t = GameObject.Instantiate(AssetManager.LoadGameObject(typeof(T).Name).GetComponent<T>());
        CanvasDic.Add(typeof(T).Name, t);
        if (typeof(T) == typeof(MainCanvas))
            CurrentMainCanvas = t;
        CreatingCanvas = false;
        return t;
    }

    public static void Clear()
    {

    }

    public static UICanvas GetCanvas(string name) 
    {
        if (CanvasDic.TryGetValue(name, out UICanvas canvas))
        {
            return canvas;
        }
        return null;
    }
    public static T GetCanvas<T>() where T : UICanvas
    {
        if (CanvasDic.TryGetValue(typeof(T).Name, out UICanvas canvas))
        {
            return (T)canvas;
        }
        return null;
    }

    public static bool RemoveCanvas(string name)
    {
        if (Views.TryGetValue(name, out var b))
        {
            Views.Remove(name);
        }
        return true;
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

            CanvasDic[old.GetType().Name] = newCanvas;
            GameObject.Destroy(old.gameObject, 0.1f);
        }
        if(newCanvas.GetType() == typeof(MainCanvas))
            CurrentMainCanvas = newCanvas;
    }
    public static bool RemoveView<T>() where T : BaseView
    {
        if (Views.TryGetValue(typeof(T).Name, out var b))
        {
            Views.Remove(typeof(T).Name);
        }
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
        if (Views.TryGetValue(name, out var b))
            return b.GetComponent<T>();
        if (create)
        {
            GameObject origin = AssetManager.LoadGameObject(name);
            if (!CanvasDic.TryGetValue(typeof(T2).Name, out UICanvas canvas))
            {
                canvas  = CreateCanvas<T2>();
            }
            T view = GameObject.Instantiate(origin, canvas.transform).GetComponent<T>();
            return view;
        }
        return null;
    }
}
