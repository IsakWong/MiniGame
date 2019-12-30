using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mini.Core;
using UnityEngine;

public class WorldLevel5 : BaseWorld
{
    // Start is called before the first frame update
    
        
        
    private int bosswordCounter = 0;
    private int endwordCounter = 0;
    
 

    protected void Awake()
    {
        base.Awake();

        OriginSpotLightIntensity = MainSpotLight.intensity;

        LevelEnemyList = MiniCore.GetConfig<LevelEnemyListConfig>("Level5EnemyList");

        WorldSequence.InsertCallback(0f,delegate()
        {
            ChangeEnvironmentLight(EnvLighColor, 1.0f);
            MainSpotLight.spotAngle = 65;
            Rain.RainIntensity = 0.4f;
            
            MiniCore.ChangeBgm("八音盒2-2",true);
            var levelBeginView = ViewManager.GetView<LevelBeginView>(true);
            levelBeginView.gameObject.SetActive(true);
            Main.SetHandLightVisible(true, 0.5f);
            CreateThunder();
            levelBeginView.PlayOutAnim(delegate ()
            {
                levelBeginView.CloseSelf();
            });

        });
        WorldSequence.Append(MainSpotLight.DOIntensity(OriginSpotLightIntensity, 1.0f));

        WorldSequence.InsertCallback(30.5f, delegate()
        {
            ChangeEnvironmentLight(new Color(0.2f,0.2f,0.2f,1), 1.0f);
        });
        WorldSequence.InsertCallback(29.5f, delegate () { CreateThunder(); });
        WorldSequence.Insert(30.0f, DOTween.To(() => MainSpotLight.spotAngle, value => MainSpotLight.spotAngle = value, 40, 3.0f));
        WorldSequence.InsertCallback(32, delegate ()
        {
            ViewManager.GetView<LevelOverlayView>().ShowBossTip1();
        });
        WorldSequence.InsertCallback(34.6f, delegate() {CreateThunder(); });
        WorldSequence.Pause();

    }

    protected void Start()
    {
        base.Start();
        MainSpotLight.intensity = 0;
        Main.SetHandLightVisible(false, -1f);
    }

    private void Callback()
    {
        MiniCore.PlaySound("Dong");
    }
    public override void ResetWorld()
    {
        base.ResetWorld();
    }
}
