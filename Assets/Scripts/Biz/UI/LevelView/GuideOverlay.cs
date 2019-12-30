using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mini.Core;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class GuideOverlay : BaseView
{
    // Start is called before the first frame update

    public Button[] Guides;
    private GuideConfig config;
    protected new void Awake()
    {
        base.Awake();
        PlayInAnim();
        config = MiniCore.GetConfig<GuideConfig>();
        foreach (var guide in Guides)
        {
            guide.onClick.AddListener(delegate()
            {
                this.Get<GameController>().Restore();
                guide.transform.parent.gameObject.SetActive(false);
                config.IsGuided[guide.transform.GetSiblingIndex()] = true;
            });
        }

    }

    public void Show(int GuideId)
    {
        foreach (var item in Guides)
        {
            if (item.gameObject.activeInHierarchy)
            {
                return;
            }
        }
        if (GuideId < config.IsGuided.Count)
        {
            this.Get<GameController>().Pause(false);
            Guides[GuideId].transform.parent.gameObject.SetActive(true);
        }

    }

    protected void OnDisable()
    {
      
    }
}