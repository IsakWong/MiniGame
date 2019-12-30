using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class DebugCanvas : UICanvas
{
    public Text DebugText;
    public Button DebugBtn;

    private StringBuilder sb = new StringBuilder(100);

    protected new void Awake()
    {
        base.Awake();
        DebugBtn.onClick.AddListener(delegate()
        {
            DebugView d = ViewManager.GetView<DebugView>(false);
            if(d!=null)
                d.gameObject.SetActive(!d.gameObject.activeInHierarchy);
            ViewManager.GetView<DebugView>(true);
        });
        name = "[Canvas.Debug]";
    }
 
    float deltaTime = 0.0f;
    // Update is called once per frame
    protected void Update()
    {
        sb.Clear();
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        sb.AppendFormat("FPS: {0:0.0}", fps);
        DebugText.text = sb.ToString();
    }
}
