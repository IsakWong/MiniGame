using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LoadingView : BaseView
{
    public RawImage LoadingImage;
    public Text TipText;
    public override void OnPlayInAnimation()
    {
        base.OnPlayInAnimation();
        LoadingImage.material.SetFloat("_Progress",1.0f);
    }

    public override void OnPlayOutAnimation()
    {
        base.OnPlayOutAnimation();
        LoadingImage.material.DOFloat(0.0f, "_Progress", 1.5f);
    }

    protected  new void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
