using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DigitalRuby.RainMaker;
using UnityEngine;

public class BaseWorld : MonoBehaviour
{
    //世界原始颜色
    public Color EnvLighColor = new Color(0.25f,0.25f,0.25f,1.0f);

    protected float OriginSpotLightIntensity;


    protected Tweener SpotLightPosTweener;
    protected Tweener SpotLightRotTweener;


    protected static Tweener EnvLightTweener;
    public static float GenerationRadius = 8f;


    public Light MainSpotLight;
    //生成范围

    public RainScript Rain;


    public bool CanBeginGame = true;

    public int Hour;
    public int Min;

    public bool HasOverlayUI = true;

    public bool WinAfterTime = true;
    public float WinTime = 20f;
    //Main主角
    public MainCharacter Main;
    //关卡开始后时间
    public float timeSinceLevelBegin = 0.0f;
    //关卡创建后时间
    public float timeSinceLevelCreate = 0.0f;

    //Enemy生成表
    protected LevelEnemyListConfig LevelEnemyList;
    //所有Enemy物体
    public HashSet<EnemyObject> Enemies = new HashSet<EnemyObject>();
    //Enemy类型表
    protected EnemyTypeConfig EnemyTypeTypeList;
    //第一次进入还是重新开始
    public bool firstBegin = true;
    //关卡世界剧本
    public Sequence WorldSequence;
    
    public void ChangeEnvironmentLight(Color target, float duration)
    {
        if (duration < 0)
        {
            RenderSettings.ambientLight = target;
            return;
            
        }
        if (EnvLightTweener == null)
        {
            EnvLightTweener = DOTween.To(() => RenderSettings.ambientLight, x => RenderSettings.ambientLight = x, target, duration).SetEase(Ease.InOutQuad).SetAutoKill(false);

        }

        if (EnvLightTweener.IsPlaying())
        {
            EnvLightTweener.Pause();
        }

        EnvLightTweener.ChangeStartValue(RenderSettings.ambientLight, duration);
        EnvLightTweener.ChangeEndValue(target, duration);
        EnvLightTweener.Restart();

    }

    public virtual void OnPause()
    {
        LogManager.Log("世界暂停");
        foreach (var enemy in Enemies)
        {
            enemy.OnPause();
        }
    }

    public virtual void OnRestore()
    {
        LogManager.Log("世界恢复");
        foreach (var enemy in Enemies)
        {
            enemy.OnRestore();
        }
    }

    #region 世界相关重载
    /// <summary>
    /// 关卡开始加载并出现
    /// </summary>
    public virtual void BeginLoadWorld()
    {
        MiniCore.Get<GameController>().IsPaused = !CanBeginGame;
        if (firstBegin)
        {
            BeginGame();
        }
        else
        {
            //重新开始游戏
            SetWorldUnVisible(delegate (BaseWorld world)
            {
                ResetWorld();
                BeginGame();
            });
        }

        LogManager.Log( GetType().ToString() + " 关卡世界开始加载并出现");
    }
    /// <summary>
    /// 开始游戏
    /// </summary>
    public virtual void BeginGame()
    {
        //展示关卡内 UI

        if (HasOverlayUI)
        {
            var overlay = ViewManager.GetView<LevelOverlayView>(false);
            if (overlay == null)
            {
                overlay = ViewManager.GetView<LevelOverlayView>(true);
            }
            overlay.gameObject.SetActive(true);
            overlay.PlayInAnim(null);

        }
        firstBegin = false;

        timeSinceLevelBegin = 0.0f;

        //启动游戏
        MiniCore.Get<GameController>().IsPaused = !CanBeginGame;

        //重新开始剧本
        WorldSequence.Restart();

        LogManager.Log(GetType().ToString() + " 关卡世界开始游戏");
    }

    public virtual void Win()
    {
        ResetWorld();
        var victory = ViewManager.GetView<LevelVictoryView>();
        victory.watch.CurHour = MiniCore.Get<GameController>().Hours[MiniCore.Get<GameController>().WorldIndex];
        victory.watch.UpdateWatch();
        victory.watch.RotateHour();
        victory.Tip.text = MiniCore.Get<GameController>().PauseTip[MiniCore.Get<GameController>().WorldIndex];
        victory.PlayInAnim();
    }
    
    /// <summary>
    /// 重置世界
    /// </summary>
    public virtual void ResetWorld()
    {
        WorldSequence.Pause();
        timeSinceLevelBegin = 0.0f;
        //重置索引
        if (LevelEnemyList != null)
            LevelEnemyList.ResetIndex();

        //重置主角
        Main.Reset();

        //回收所有Enemy
        foreach (var enemy in Enemies)
        {
            enemy.CurrentState = EnemyObject.ObjectState.Obsorbed;
        }

        LogManager.Log(GetType().ToString() + " 重置");
    }
    

    public virtual void SetWorldUnVisible(Action<BaseWorld> callback,float duration = 1.0f)
    {
        if (SpotLightRotTweener != null)
            SpotLightRotTweener.Kill(false);

        Main.SetHandLightVisible(false, duration);

        SpotLightRotTweener = MainSpotLight.transform.DORotate(new Vector3(0, 0, 0), duration).OnComplete(
            delegate ()
            {
                if (callback != null)
                    callback.Invoke(this);
            });

        MainSpotLight.DOIntensity(0, duration);
        ChangeEnvironmentLight(Color.black, duration);

        var mainMenu = ViewManager.GetView<MainMenu>(false);
        if (mainMenu != null)
            mainMenu.PlayOutAnim(null);
    }
    #endregion


    public T CreateEnemyObjectWithoutInit<T>(string prefabName) where T: EnemyObject
    {

        T obj = ObjectManager.CreateManagedObject(prefabName).GetComponent<T>();
        if (!Enemies.Contains(obj))
            Enemies.Add(obj);
        return obj;
    }
    #region 世界相关方法
    protected void GenerateEnemyByList()
    {
        if (LevelEnemyList == null)
            return;

        List<EnemyGenerateInfo> list = LevelEnemyList.GetEnemyByTime(timeSinceLevelBegin);
        if (list != null)
        {
            if (list.Count > 0)
            {
                EnemyInfo typeInfo = null;
                foreach (var info in list)
                {
                    typeInfo = EnemyTypeTypeList.GetEnemyInfoByIndex(info.Type);
                    EnemyObject obj = ObjectManager.CreateManagedObject(typeInfo.PrefabName).GetComponent<EnemyObject>();
                    switch (info.EnemyMoveType)
                    {
                        case MoveType.Line:
                            obj.CachedRigidbody.angularVelocity = info.AngleVelocity * MiniCore.TimeScale;
                            obj.CachedRigidbody.angularDrag = info.AngleDrag;
                            obj.CachedRigidbody.velocity = new Vector2(Mathf.Cos(info.VelocityDirectionAngle * Mathf.Deg2Rad) * info.Velocity, Mathf.Sin(info.VelocityDirectionAngle * Mathf.Deg2Rad) * info.Velocity) * MiniCore.TimeScale;
                            obj.CachedRigidbody.drag = info.Drag;
                            break;
                        case MoveType.Curve:
                            break;
                        case MoveType.Specific:
                            break;
                        case MoveType.Unknown:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    obj.transform.position = new Vector3(Mathf.Cos(info.Angle * Mathf.Deg2Rad) * GenerationRadius, Mathf.Sin(info.Angle * Mathf.Deg2Rad) * GenerationRadius, 0);
                    obj.Init();
                    if (!Enemies.Contains(obj))
                        Enemies.Add(obj);
                }
            }
        }

    }
    public void CreateThunder()
    {
        ObjectManager.CreateManagedObject("ThunderEffect");
    }

    public virtual void DestroyWorld(Action destroyedCallback)
    {
        
        WorldSequence.Pause();
        if (LevelEnemyList != null)
            LevelEnemyList.ResetIndex();
        if (HasOverlayUI)
        {
            var overlay = ViewManager.GetView<LevelOverlayView>(false);
            if(overlay != null)
                overlay.PlayOutAnim(null);
        }
        foreach (var enemy in Enemies)
        {
            enemy.Recycle();
        }
        SetWorldUnVisible(delegate (BaseWorld world)
        {
            
            Destroy(world.gameObject);
            EnvLightTweener.Pause();
            WorldSequence.Kill(false);
            destroyedCallback.Invoke();
        });
    }

    #endregion
    protected void Start()
    {
        LogManager.Log(GetType().ToString() + " Start.");
    }

    private void OnHealthChange(int old, int newVal)
    {
        if(MiniCore.Get<GameController>().IsPaused)
            return;
        if (newVal < old)
        {
            ViewManager.GetView<LevelOverlayView>().DoRedFade();
        }
    }

    protected void Awake()
    {
        if (WorldSequence == null)
            WorldSequence = DOTween.Sequence();
        Rain = GameObject.Find("Rain").GetComponent<RainScript>();
        WorldSequence.SetAutoKill(false).Pause();
        EnemyTypeTypeList = MiniCore.GetConfig<EnemyTypeConfig>("EnemyTypeList");
        this.AddListener<int, int>(GlobalGameMessage.OnHealthChange, OnHealthChange);
        LogManager.Log(GetType().ToString() + " 关卡世界初始化");
    }


    protected void OnDestroy()
    {
        this.RemoveListener<int,int>(GlobalGameMessage.OnHealthChange,OnHealthChange);
    }

    protected void FixedUpdate()
    {
       
        if (!MiniCore.Get<GameController>().IsPaused)
        {
            timeSinceLevelBegin += Time.fixedDeltaTime * MiniCore.TimeScale;
            GenerateEnemyByList();
            if (timeSinceLevelBegin > WinTime && WinTime > 0 && WinAfterTime)
            {
                MiniCore.Get<GameController>().Win();
            }
        }

        foreach (var enemy in Enemies)
        {
            if (enemy.gameObject.activeSelf)
            {
                if (enemy.transform.position.magnitude > 12)
                {
                    enemy.transform.position = new Vector3(8,0,0);
                    enemy.Recycle();
                }
            }
            
        }
        timeSinceLevelCreate += Time.fixedDeltaTime;
    }

 
}

