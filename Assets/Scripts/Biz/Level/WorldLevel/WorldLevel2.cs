using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mini.Core;
using UnityEngine;

public class WorldLevel2 : BaseWorld
{
    // Start is called before the first frame update
    
        
    

    protected void Awake()
    {
        base.Awake();

        MiniCore.ChangeBgm("雨夜-02");
        OriginSpotLightIntensity = MainSpotLight.intensity;

        LevelEnemyList = MiniCore.GetConfig<LevelEnemyListConfig>("Level2EnemyList");

        WorldSequence.AppendCallback(delegate()
        {
            Rain.RainIntensity = 0.4f;
            var levelBeginView = ViewManager.GetView<LevelBeginView>(true);
            Main.SetHandLightVisible(true, 0.5f);
            CreateThunder();
            levelBeginView.gameObject.SetActive(true);
        });
        WorldSequence.AppendInterval(0.1f);
        WorldSequence.AppendCallback(delegate()
        {
            var levelBeginView = ViewManager.GetView<LevelBeginView>(true);
            levelBeginView.gameObject.SetActive(false);
        });
        WorldSequence.AppendInterval(0.4f);
        WorldSequence.AppendCallback(delegate ()
        {
            var levelBeginView = ViewManager.GetView<LevelBeginView>(true);
            levelBeginView.gameObject.SetActive(true);
            CreateThunder();
        });
        WorldSequence.AppendInterval(0.1f);
        WorldSequence.AppendCallback(delegate ()
        {
            ChangeEnvironmentLight(EnvLighColor,1.0f);
            var levelBeginView = ViewManager.GetView<LevelBeginView>(true);
            levelBeginView.PlayOutAnim(delegate()
            {
                levelBeginView.CloseSelf();
            });

        });
        WorldSequence.Append(MainSpotLight.DOIntensity(OriginSpotLightIntensity, 1.0f));
        WorldSequence.InsertCallback(13f, delegate() { CreateThunder(); });

        WorldSequence.Pause();

    }

    protected void Start()
    {
        base.Start();
        MainSpotLight.intensity = 0;
        Main.SetHandLightVisible(false, -1f);
    }
    
}
