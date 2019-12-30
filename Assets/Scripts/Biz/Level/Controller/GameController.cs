using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using DG.Tweening.Plugins;
using Mini.Core;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using Random = UnityEngine.Random;

public class GameController : BaseController
{

    public string[] Worlds =
  {
        "WorldLevel0",
        "WorldLevel1",
        "WorldLevel2",
        "WorldLevel3",
        "WorldLevel4",
        "WorldLevel5",
        "WorldLevel6"
    };

    public string[] Tips =
    {
        "用蓝臂抵挡蓝色飞行物，用黄臂吸收黄色光圈的道具。",
        "用蓝臂抵挡蓝色飞行物，用黄臂吸收黄色光圈的道具。",
        "飞来的蓝色物体都充满恶意，黄色的糖果和小熊会帮助你。",
        "身体和黄臂容易受到攻击，小心翼翼地旋转，不要让蓝色飞行物碰到。",
        "用蓝臂成功抵御会积攒能量，攒满后点击少女可释放强力技能。",
        "星星魔法棒能释放光芒，消除周围的飞行物。",
        "故事走向了尾声"
    };

    public string[] PauseTip =
    {
        "滴答滴答，雨一直下，独舞的少女，没有自己的家。",
        "时间在靠近，雷雨在轰鸣，空荡的教室里，传来一阵脚步声音。",
        "少女在哭泣，那些欺凌谩骂之人如同恶魔，正在舔舐她脆弱的心！",
        "八音盒的美妙旋律，让少女进入自己的内心世界。",
        "直面内心的恐惧，让梦魇化作一阵温雨，洒向漆黑的大地。",
        "直面内心的恐惧，让梦魇化作一阵温雨，洒向漆黑的大地。",
        "直面内心的恐惧，让梦魇化作一阵温雨，洒向漆黑的大地。"
    };
    public int[] Hours =
    {
       0,
       21,
       22,
       23,
       0,
       3,
       6,
    };

    public int WorldIndex = 0;

    private float _Time = 0;
    // Start is called before the first frame update

    public BaseWorld CurrentWorld = null;

    private bool _isPaused = true;


    public bool IsPaused
    {
        get { return _isPaused; }
        set
        {
            _isPaused = value;
        }
    }

    public bool IsLoading = true;


    private MainCharacter mainCharacter;


    #region 游戏流程控制公共接口


    public void Restore()
    {


        MiniCore.CoreBehaviour.GlobalBgmAudio.pitch = 1.0f;
        CurrentWorld.WorldSequence.timeScale = 1.0f;
        CurrentWorld.OnRestore();
        IsPaused = false;

        LevelPauseView level = ViewManager.GetView<LevelPauseView>(false);
        if (level != null)
            level.PlayOutAnim(null);
    }
    public void Pause(bool showPauseView = true)
    {
        MiniCore.CoreBehaviour.GlobalBgmAudio.pitch = 0.0f;
        CurrentWorld.WorldSequence.timeScale = 0.0f;
        CurrentWorld.OnPause();
        IsPaused = true;

        if (showPauseView)
        {
            LevelPauseView level = ViewManager.GetView<LevelPauseView>(true);
            level.PlayInAnim(null);
        }
        

    }
    public void Defeat()
    {
        IsPaused = true;

        var defeatView = ViewManager.GetView<LevelDefeatView>(true);
        defeatView.PlayInAnim(null);

        var guideView = ViewManager.GetView<GuideOverlay>(false);
        if (guideView != null)
            guideView.PlayOutAnim(delegate ()
            {
                ViewManager.GetView<GuideOverlay>().CloseSelf();
            });
        var pauseView = ViewManager.GetView<LevelPauseView>(false);
        if (pauseView != null)
            pauseView.PlayOutAnim(null);

        CurrentWorld.ResetWorld();
    }

    public void LoadWorld(int index)
    {
        if (index >= Worlds.Length)
            index = 0;
        IsPaused = true;
        WorldIndex = index;
        if (CurrentWorld != null)
        {
            IsLoading = true;
            var loadingView = ViewManager.GetView<LoadingView>(true);
            loadingView.PlayInAnim(null);
            loadingView.TipText.text = Tips[WorldIndex];
            MiniCore.CallLoadAsset(Worlds[WorldIndex], OnWorldLoading);
            CurrentWorld.DestroyWorld(delegate ()
            {
                CurrentWorld = null;
            });
        }
    }
    public void BackToMenu()
    {
        IsPaused = true;
        var defeatView = ViewManager.GetView<LevelDefeatView>(false);
        /*
        defeatView.PlayOutAnim(delegate ()
        {
            defeatView.CloseSelf();
        });
        */
        LoadWorld(0);
    }

    public void Win()
    {
        IsPaused = true;
        ViewManager.GetView<GuideOverlay>().PlayOutAnim(delegate ()
        {
            ViewManager.GetView<GuideOverlay>().CloseSelf();
        });
        CurrentWorld.Win();
    }
    public void Restart()
    {
        CharacterController characterController = MiniCore.Get<CharacterController>();

        var defeatView = ViewManager.GetView<LevelDefeatView>(false);
        defeatView.PlayOutAnim(delegate()
        {
            defeatView.CloseSelf();
        });

        characterController.Score = 0;
        characterController.CurrentHealth = 3;

        CurrentWorld.BeginLoadWorld();
    }


    public void StartGame()
    {

        CharacterController characterController = MiniCore.Get<CharacterController>();
        characterController.CurrentHealth = 3;

        CurrentWorld.BeginLoadWorld();


    }

    public void NextWorld()
    {
        LoadWorld(WorldIndex + 1);
    }

    private bool OnWorldLoading(bool loaded, float arg2)
    {
        if (loaded)
        {
            if (CurrentWorld == null)
            {
                CurrentWorld = GameObject.Instantiate(AssetManager.LoadAsset<GameObject>(Worlds[WorldIndex])).GetComponent<BaseWorld>();
                mainCharacter = GameObject.Find("Main").GetComponent<MainCharacter>();

                HashSet<string> list = MiniCore.GetConfig<WorldCacheAssetConfig>().GetWorldCacheAssetsByIndex(WorldIndex);
                if (list == null)
                {
                    OnWorldAssetLoading(true, 1);
                    return true;
                }

                int i = 0;
                foreach (var assetName in list)
                {
                    if (i == list.Count - 1)
                        MiniCore.CallLoadAsset(assetName, OnWorldAssetLoading);
                    else
                        MiniCore.CallLoadAsset(assetName, null);

                    i++;
                }


                return true;

            }
            return false;
        }
        return false;
    }
    private bool OnWorldAssetLoading(bool loaded, float arg2)
    {
        if (loaded)
        {
            IsLoading = false;
            var loadingView = ViewManager.GetView<LoadingView>(true);
            loadingView.PlayOutAnim(delegate ()
            {
                StartGame();
            });
            return true;
        }
        return false;
    }

    public void InitGame()
    {
        LoadingView loadingView = ViewManager.GetView<LoadingView>();
        loadingView.PlayInAnim(delegate()
        {
            MiniCore.CallLoadAsset("main_menu_bgm", null);
            MiniCore.CallLoadAsset(Worlds[0], OnFirstWorldLoading);


        });
    }
    private bool OnFirstWorldLoading(bool isLoaded, float progress)
    {
        if (isLoaded)
        {

            CurrentWorld = GameObject.Instantiate(AssetManager.LoadAsset<GameObject>("WorldLevel0")).GetComponent<WorldLevel0>();

            mainCharacter = GameObject.Find("Main").GetComponent<MainCharacter>();

            //spotLight = GameObject.Find("SpotLight").GetComponent<Light>();
            //spotLight.DOIntensity(0, 2.0f).SetLoops(-1, LoopType.Yoyo);

            IsPaused = true;
            LoadingView loadingView = ViewManager.GetView<LoadingView>();
            loadingView.PlayOutAnim(delegate ()
            {
                loadingView.CloseSelf();
                CurrentWorld.BeginLoadWorld();
            });
            return true;
        }

        return false;

    }

    #endregion

    public override void Init()
    {
        MessageManager.AddListener(null, GlobalGameMessage.OnLevelOver, Defeat);
        EnableFixedUpdate = true;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        foreach (var enemy in EnemyObject.EnemyObjects)
        {
            if (enemy.CachedRigidbody != null)
            {
                if (enemy.CachedRigidbody.velocity.magnitude > enemy.MaxVelocitySize)
                {
                    enemy.CachedRigidbody.velocity = enemy.CachedRigidbody.velocity / enemy.CachedRigidbody.velocity.magnitude * enemy.MaxVelocitySize;
                }
                if (enemy.CachedRigidbody.angularVelocity > enemy.MaxAngularVelocity)
                {
                    enemy.CachedRigidbody.angularVelocity = enemy.MaxAngularVelocity;
                }
            }
        }
    }

}
