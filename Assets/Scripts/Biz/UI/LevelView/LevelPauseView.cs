using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using DG.Tweening;
using Mini.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LevelPauseView : BaseView
{
    // Start is called before the first frame update

    public Button Mute;
    public Button BackToMenu;
    public WheelWidget Wheel;

    public Texture2D[] textures;

    public override void OnPlayInAnimation()
    {
        base.OnPlayInAnimation();
        var v = (Wheel.transform as RectTransform).anchoredPosition;
        v.y = -402;
        (Wheel.transform as RectTransform).anchoredPosition = v;
        (Wheel.transform as RectTransform).DOAnchorPosY(0, 0.5f);

    }

    public override void OnPlayOutAnimation()
    {
        base.OnPlayOutAnimation();
    }

    protected void OnDestroy()
    {
        base.OnDestroy();
        Wheel.OnTrigger = null;
    }
    protected new void Start()
    {
        base.Start();
        PlayInAnim();
        Wheel.OnTrigger = delegate ()
        {
            var overlay = ViewManager.GetView<LevelOverlayView>(false);
            if (overlay != null)
            {
                (overlay.HeartGroup.transform as RectTransform).DOAnchorPosY(57, 0.5f);
                overlay.Black.DOColor(new Color(0, 0, 0, 0), 0.5f);
                RectTransform rect = overlay.PauseBtn.transform as RectTransform;
                rect.DOAnchorPosY(-117, 0.5f);
            }
            PlayOutAnim(delegate()
            {
                MiniCore.Get<GameController>().Restore();
                overlay.PauseBtn.interactable = true;
                CloseSelf();
            });
         
            
        };
        Mute.onClick.AddListener(delegate ()
        {
            Camera.main.GetComponent<AudioListener>().enabled = !Camera.main.GetComponent<AudioListener>().enabled;
            MiniCore.Get<GameController>().Restore();
            var overlay = ViewManager.GetView<LevelOverlayView>(false);
            if (overlay != null)
            {
                (overlay.HeartGroup.transform as RectTransform).DOAnchorPosY(57, 0.5f);
                overlay.Black.DOColor(new Color(0, 0, 0, 0), 0.5f);
                RectTransform rect = overlay.PauseBtn.transform as RectTransform;
                rect.DOAnchorPosY(-117, 0.5f);
                overlay.PauseBtn.interactable = true;
            }
            PlayOutAnim(delegate ()
            {
                CloseSelf();
            });

        });

        BackToMenu.onClick.AddListener(delegate ()
        {
            
            MiniCore.Get<GameController>().CurrentWorld.ResetWorld();
            MiniCore.Get<GameController>().BackToMenu();
            MiniCore.Get<GameController>().Restore();
            var overlay = ViewManager.GetView<LevelOverlayView>(false);
            if (overlay != null)
            {
                (overlay.HeartGroup.transform as RectTransform).DOAnchorPosY(57, 0.5f);
                overlay.Black.DOColor(new Color(0, 0, 0, 0), 0.5f);
                RectTransform rect = overlay.PauseBtn.transform as RectTransform;
                rect.DOAnchorPosY(-117, 0.5f);
                overlay.PauseBtn.interactable = true;
            }
            PlayOutAnim(delegate ()
            {
                CloseSelf();
            });
        });

    }

}
