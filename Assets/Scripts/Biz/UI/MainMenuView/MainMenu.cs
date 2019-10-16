using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class MainMenu : BaseView
{
    // Start is called before the first frame update

    public Button Left;
    public Button Right;
    public ScrollRect scrollView;
    public Watch[] Watches;
    public WheelWidget wheel;
    public int index = 0;
    public override void OnPlayInAnimation()
    {
        base.OnPlayInAnimation();
        wheel.Start.DOAnchorPosX(0, 0.5f);
        MiniCore.ChangeBgm("main_menu_bgm", true);
    }
    
    protected new void Awake()
    {
        base.Awake();
        wheel.OnTrigger = delegate ()
        {
            if (this.gameObject.activeInHierarchy)
            {
                PlayOutAnim(delegate ()
                {
                    this.Get<GameController>().LoadWorld(index + 1);
                    this.gameObject.SetActive(false);
                });
            }
           
        };
        Left.onClick.AddListener(delegate()
        {
            if (index > 0)
            {
                index--;
                scrollView.DOHorizontalNormalizedPos(  (float) index / (float)(scrollView.content.childCount - 3), 0.5f);
            }
            

        });
        Right.onClick.AddListener(delegate()
        {
            if (index < scrollView.content.childCount - 3)
            {
                index++;
                scrollView.DOHorizontalNormalizedPos((float)index / (float)(scrollView.content.childCount - 3), 0.5f);
            }

        });
        this.AddListener(GlobalGameMessage.OnLevelBegin, delegate(object data) { MiniCore.Get<GameController>(); });

    }

    private void Update()
    {
        for (int i = 0; i < Watches.Length; ++i)
        {
            RectTransform rt = Watches[i].transform as RectTransform;
            Vector3 pos = rt.position;
            float alpha = 1 - Mathf.Abs(pos.x - 540.0f) / 250.0f;
            Watches[i].SetAlpha(alpha);
        }
    }
}