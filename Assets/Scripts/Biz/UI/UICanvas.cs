using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICanvas : BaseView
{
    protected new void Awake()
    {
        if (ViewManager.Instance.CreatingCanvas == false)
        {
            if (ViewManager.GetCanvas(GetType().Name) != this)
            {
                ViewManager.CombineCanvas(this);
            }
        }
    }

    protected new void OnDestroy()
    {
        Debug.Log(gameObject);
        ViewManager.Instance.CanvasDictionary.Remove(GetType().Name);
    }
}
