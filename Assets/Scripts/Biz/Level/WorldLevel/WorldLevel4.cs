using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WorldLevel4 : BaseWorld
{
    protected void Awake()
    {
        base.Awake();
        
        OriginSpotLightIntensity = MainSpotLight.intensity;

        LevelEnemyList = MiniCore.GetConfig<LevelEnemyListConfig>("Level4EnemyList");
        WorldSequence.InsertCallback(0f,delegate()
        {
            Rain.RainIntensity = 0.5f;
            MiniCore.ChangeBgm("八音盒2-1",true);
            var levelBeginView = ViewManager.GetView<LevelBeginView>(true);
            levelBeginView.gameObject.SetActive(true);
            Main.SetHandLightVisible(true, 0.5f);
            CreateThunder();
        });

        WorldSequence.InsertCallback(0.1f, delegate ()
        {
            var levelBeginView = ViewManager.GetView<LevelBeginView>(true);
            levelBeginView.gameObject.SetActive(false);
        });
        WorldSequence.InsertCallback(1.0f,delegate ()
        {
            ChangeEnvironmentLight(EnvLighColor,1.0f);
            var levelBeginView = ViewManager.GetView<LevelBeginView>(true);
            levelBeginView.PlayOutAnim(delegate()
            {
                levelBeginView.CloseSelf();
            });
        });

        WorldSequence.Insert(1.0f, MainSpotLight.DOIntensity(OriginSpotLightIntensity, 1.0f));
        WorldSequence.InsertCallback(1.09f, delegate ()
        {
            this.Get<CharacterController>().AbilityProgress = 100;
        });
        
        WorldSequence.InsertCallback(1.1f, delegate ()
        {
            ViewManager.GetView<GuideOverlay>().Show(4);
        });
        WorldSequence.InsertCallback(21f, delegate () { CreateThunder(); });
        WorldSequence.InsertCallback(41.5f, delegate () { CreateThunder(); });

        WorldSequence.Pause();

    }

    protected void Start()
    {
        base.Start();
        MainSpotLight.intensity = 0;
        Main.SetHandLightVisible(false, -1f);
    }

}
