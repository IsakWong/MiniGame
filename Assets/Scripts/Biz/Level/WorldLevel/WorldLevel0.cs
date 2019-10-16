using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WorldLevel0 : BaseWorld
{
    private int loadcount = 0;
    protected void Awake()
    {
        base.Awake();

        WorldSequence.AppendCallback(delegate()
        {
            if (loadcount != 0)
            {
                Rain.RainIntensity = 1.0f;
                MiniCore.ChangeBgm("main_menu_bgm", true);
            }
            loadcount++;

            var menu = ViewManager.GetView<MainMenu>(false);
            if (menu == null)
            {
                ViewManager.GetView<MainMenu>(true).gameObject.SetActive(false);
            }
            else
            {
                menu.gameObject.SetActive(true);
            }
            menu.PlayInAnim();
            ChangeEnvironmentLight(EnvLighColor, 0.5f);
            Main.SetHandLightVisible(true, 0.5f);
            MainSpotLight.DOIntensity(OriginSpotLightIntensity, 0.5f);

        });
        WorldSequence.AppendCallback(delegate ()
        {
            SpotLightRotTweener = MainSpotLight.transform.DORotate(new Vector3(0, 25, 0), 3.2f)
                .SetEase(Ease.InOutCubic).SetLoops(-1, LoopType.Yoyo);
        });
    }

    protected void Start()
    {
        base.Start();
        OriginSpotLightIntensity = MainSpotLight.intensity;
        ChangeEnvironmentLight(Color.black, -1f);
        Main.SetHandLightVisible(false, -1);
        //spotLight.intensity = 0;
    }

}
