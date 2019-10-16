using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Story1_3 : BaseView
{
    public Image TipText;
    public Button btn;
    public override void OnPlayInAnimation()
    {
        base.OnPlayInAnimation();
    }

    public void Thunder()
    {
        this.Get<GameController>().CurrentWorld.CreateThunder();
    }

    public void Close()
    {
        this.Get<GameController>().Win();
        PlayOutAnim(delegate ()
        {
            CloseSelf();
        });
    }
    protected void Awake()
    {
        base.Awake();
        Invoke("Thunder",1.0f);
        Invoke("Thunder", 4.0f);
        Invoke("Close", 7.0f);
    }

    protected void OnDestroy()
    {
        base.OnDestroy();
    }

}
