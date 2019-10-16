using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Story0 : BaseView
{
    public Text TipText;
    public Button btn;
    public override void OnPlayInAnimation()
    {
        base.OnPlayInAnimation();
    }

    protected void Awake()
    {
        base.Awake();
        btn.onClick.AddListener(delegate()
        {
            PlayOutAnim(delegate ()
            {
                ViewManager.GetView<MainMenu>().gameObject.SetActive(true);
                ViewManager.GetView<MainMenu>().PlayInAnim();
                CloseSelf();
            });
        });

    }

    protected void OnDestroy()
    {
        base.OnDestroy();
        btn.onClick.RemoveAllListeners();
    }

}
