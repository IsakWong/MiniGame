using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WorldLevel1 : BaseWorld
{
    // Start is called before the first frame update

        
    

    protected void Awake()
    {
        base.Awake();
        MiniCore.ChangeBgm("雨夜-01");
        OriginSpotLightIntensity = MainSpotLight.intensity;

        LevelEnemyList = MiniCore.GetConfig<LevelEnemyListConfig>("Level1EnemyList");

        WorldSequence.InsertCallback(1.0f, delegate()
        {
            Rain.RainIntensity = 0.3f;
            var levelBeginView = ViewManager.GetView<LevelBeginView>(true);
            levelBeginView.PlayInAnim(delegate()
            {
                levelBeginView.PlayOutAnim(delegate() { levelBeginView.CloseSelf(); });
            });

            this.Main.SetHandLightVisible(true, 0.25f);
            ChangeEnvironmentLight(EnvLighColor, 0.1f);
            MainSpotLight.DOIntensity(OriginSpotLightIntensity, 0.1f);
            MiniCore.PlaySound("Dong");

        });
        WorldSequence.InsertCallback(6.5f, delegate() {
            CreateThunder();
        });
        WorldSequence.InsertCallback(2.5f,delegate()
        {
            ViewManager.GetView<GuideOverlay>().Show(0);
        });
        WorldSequence.InsertCallback(4.5f, delegate ()
        {
            ViewManager.GetView<GuideOverlay>().Show(1);
        });
        WorldSequence.InsertCallback(17f, delegate ()
        {
            ViewManager.GetView<GuideOverlay>().Show(2);
        });
        WorldSequence.InsertCallback(23f, delegate ()
        {
            ViewManager.GetView<GuideOverlay>().Show(3);
        });
        WorldSequence.Pause();

    }
    protected void Start()
    {
        base.Start();
        Main.SetHandLightVisible(false,-1f);
        MainSpotLight.intensity = 0;
    }
    
}
