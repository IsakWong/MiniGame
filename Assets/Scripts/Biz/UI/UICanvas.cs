using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICanvas : BaseView
{
    protected new void Awake()
    {
        if (ViewManager.CreatingCanvas == false)
        {
            if (ViewManager.GetCanvas(GetType().Name) != this)
            {
                ViewManager.CombineCanvas(this);
            }
        }
    }
}
