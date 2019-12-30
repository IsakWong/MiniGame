using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mini.Core;
using UnityEngine;

public class WorldLevel3 : BaseWorld
{
    public GameObject bg1;
    public GameObject bg2;

    public GameObject finalLight;
    private GameObject wordsPrefab;
    private GameObject words;

    public FunnyFaceBoss boss;

    public Transform WheelTrans;
    public SpriteRenderer OldWorld;
    public SpriteRenderer NewWorld;

    public SpriteRenderer AbilityTip;

    public Tweener _abilityTip;
    public override void ResetWorld()
    {
        base.ResetWorld();

        OldWorld.color = new Color(1, 1, 1, 1);
        NewWorld.color = new Color(0, 0, 0, 0);
        WheelTrans.localScale = new Vector3(0, 0.32f, 1);

        _abilityTip.Kill();
        AbilityTip.color = new Color(1, 1, 1, 0);
        GameObject.Destroy(words);

        this.RemoveListener(GlobalGameMessage.OnAbilityTrigger, AbilityTriggered);
        words = null;
        finalLight.SetActive(false);
    }

  
    public override void OnPause()
    {
        base.OnPause();

    }

    public override void OnRestore()
    {
        base.OnRestore();
    }

    public void AutoWin()
    {
        base.ResetWorld();
        this.Get<GameController>().IsPaused = true;
        ViewManager.GetView<Story1_3>().PlayInAnim();
    }

    protected void Awake()
    {
        base.Awake();
        OriginSpotLightIntensity = MainSpotLight.intensity;

        LevelEnemyList = MiniCore.GetConfig<LevelEnemyListConfig>("Level3EnemyList");
        this.AddListener(GlobalGameMessage.OnAbilityTrigger, delegate ()
         {
             ChangeEnvironmentLight(EnvLighColor, 0.3f);
             DOTween.To(() => MainSpotLight.spotAngle, x => MainSpotLight.spotAngle = x, 80, 0.3f);
         });


        WorldSequence.InsertCallback(0.0f,delegate()
        {
            MiniCore.ChangeBgm("雨夜-03", true);
            Rain.RainIntensity = 0.7f;

            var levelBeginView = ViewManager.GetView<LevelBeginView>(true);
            levelBeginView.PlayInAnim(delegate ()
            {
                levelBeginView.PlayOutAnim(delegate ()
                {
                    levelBeginView.CloseSelf();
                });
            });

            MainSpotLight.transform.position = new Vector3(0,0,-8.25f);
            MainSpotLight.DOIntensity(OriginSpotLightIntensity, 1f);

            MiniCore.PlaySound("聚光灯通电");
            ChangeEnvironmentLight(EnvLighColor, 1.0f);
            Main.SetHandLightVisible(true, 0.5f);
            
        });
        WorldSequence.InsertCallback(15.0f, delegate()
        {

            GameObject maincanvas = GameObject.FindGameObjectWithTag("Canvas");
            wordsPrefab = AssetManager.LoadGameObject("Words");
            words = GameObject.Instantiate(wordsPrefab);

            words.transform.SetParent(maincanvas.transform);
            Vector3 pos = new Vector3(0, 0, 0);
            words.transform.localPosition = pos;
        });

        WorldSequence.InsertCallback(2.5f,delegate () {
            MiniCore.PlaySound("Dong");
            MainSpotLight.intensity = 0;
            ChangeEnvironmentLight(Color.black, 0.1f);
        });
   
        WorldSequence.Insert(2.5f,MainSpotLight.DOIntensity(0, 0.1f));

        WorldSequence.InsertCallback(4f,delegate () {
            MiniCore.PlaySound("Dong");
            MainSpotLight.intensity = OriginSpotLightIntensity;
            MainSpotLight.transform.position = new Vector3(0, 7.28f, -8.25f);
        });

        WorldSequence.InsertCallback(5.5f, delegate ()
        {
            MiniCore.PlaySound("Dong");
            MainSpotLight.intensity = 0;
            MainSpotLight.transform.position = new Vector3(0, 0f, -8.25f);
        });

        WorldSequence.InsertCallback(6.5f, delegate ()
        {
            MiniCore.PlaySound("Dong");
            MainSpotLight.intensity = OriginSpotLightIntensity;
            MainSpotLight.transform.position = new Vector3(0, 0f, -8.25f);
            ChangeEnvironmentLight(EnvLighColor, 1.0f);
        });
        WorldSequence.InsertCallback(34,delegate()
        {
            MiniCore.PlaySound("Dong");
            MiniCore.PlaySound("黑色触手出场");
        });

        float handTime = 40;
        WorldSequence.InsertCallback(handTime, delegate ()
        {
            foreach(EnemyObject enemy in Enemies)
            {
                if(enemy.GetType() == typeof(HandEnemy))
                {
                    HandEnemy hand = enemy as HandEnemy;
                    hand.tmp = true;
                }
            }
            boss.Stop();
        });
        WorldSequence.Insert(handTime, MainSpotLight.transform.DOMoveY(Main.transform.position.y, 2.0f));
        WorldSequence.InsertCallback(handTime, delegate ()
        {
            ChangeEnvironmentLight(Color.black, 2.0f);
            
        });
        WorldSequence.Insert(handTime + 2, DOTween.To(()=>MainSpotLight.spotAngle,x => MainSpotLight.spotAngle = x,10,2f));
        WorldSequence.InsertCallback(handTime + 2, delegate ()
         {
             MiniCore.PlaySound("黑色触手出场");
         });
        WorldSequence.InsertCallback(handTime + 2 + 2, delegate ()
         {
             this.Get<CharacterController>().AbilityProgress = 100;
             this.AddListener(GlobalGameMessage.OnAbilityTrigger, AbilityTriggered);
             finalLight.SetActive(true);
         });
        WorldSequence.InsertCallback(handTime + 4,delegate()
        {
            _abilityTip = AbilityTip.DOColor(new Color(1, 1, 1, 1), 0.3f).SetLoops(-1, LoopType.Yoyo);
        });
        //10秒后强制启动技能
        WorldSequence.InsertCallback(handTime + 2 + 4+10, delegate ()
        {
            Main.CreateMagicField();
        });
        WorldSequence.Pause();

    }

    public void ChangeWorld()
    {
        CreateThunder();
        OldWorld.color = new Color(0, 0, 0, 0);
        NewWorld.color = new Color(1, 1, 1, 1);
        WheelTrans.localScale = new Vector3(0.32f,0.32f,1);
        
    }
    public void AbilityTriggered()
    {
        ChangeEnvironmentLight(EnvLighColor, 1f);
        DOTween.To(() => MainSpotLight.spotAngle, x => MainSpotLight.spotAngle = x, 80, 1f);

        Invoke("ChangeWorld", 2);
        Invoke("AutoWin", 4);
    }
    protected void Start()
    {
        base.Start();
        MainSpotLight.intensity = 0;
        Main.SetHandLightVisible(false, -1f);
    }
    
    
}
