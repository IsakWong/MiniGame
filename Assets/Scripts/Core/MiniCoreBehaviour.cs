using System;
using System.Collections;
using System.IO;
using DG.Tweening;
using Mini.Core;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class MiniCoreBehaviour : MonoBehaviour
{

    public bool UseAsyncLoading = true;
    public bool SaveCache = true;
    public AudioSource GlobalBgmAudio = null;

    private Tweener _bgmTweener;
    private Tweener bgmTweener2;

    void Awake()
    {


        Application.logMessageReceived += HandleLog;

        AssetManager.Instance.Init();
        var settings = MiniCore.GetConfig<SettingsConfig>();
        #region Screen 设置

#if UNITY_EDITOR
        Screen.fullScreen = false;
        Application.targetFrameRate = -1;
#else
        Application.targetFrameRate = 60;
#endif
        #endregion


        #region 初始化Canvas

        MainCanvas canvas = GameObject.FindObjectOfType<MainCanvas>();
        if (canvas != null)
        {
            ViewManager.Instance.CanvasDic.Add(typeof(MainCanvas).Name, canvas);
            ViewManager.Instance.CurrentMainCanvas = canvas;
        }

        #endregion

        #region Init Mini Core
        DOTween.Init(false, true, LogBehaviour.ErrorsOnly);

        GlobalBgmAudio = GetComponent<AudioSource>();
        _bgmTweener = GlobalBgmAudio.DOFade(0f, 0.5f).SetAutoKill(false).Pause();

        Debug.developerConsoleVisible = true;
        if (MiniCore.CoreBehaviour != null)
        {
            Destroy(this);
            LogManager.Log("{0} Error:不能同时存在两个Core Behaviour。");
        }
        else
            MiniCore.CoreBehaviour = this;

        LogManager.Log("Mini Core Behaviour唤醒");
        #endregion


        #region DEBUG Canvas

        ViewManager.CreateCanvas<DebugCanvas>();

        #endregion


    }

    #region Log副处理
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        DebugView.CacheLog(">" + logString);
    }
    #endregion

    void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;

#if UNITY_EDITOR

        if (MiniCore.CoreBehaviour.SaveCache)
        {
            string path = "Assets/Resources/Generated/WorldCacheAssetConfig.asset";
            WorldCacheAssetConfig config = AssetDatabase.LoadAssetAtPath<WorldCacheAssetConfig>(path);
            if (config != null)
                config.Save();
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
        }

#endif

        if (MiniCore.CoreBehaviour == this)
        {
            AssetManager.Unload();
        }

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Vector3.zero, 8f);
    }

    public void ChangeBGM(string newBgmName,bool forceChange)
    {
        if (!forceChange)
        {
            if (GlobalBgmAudio.clip == AssetManager.LoadAsset<AudioClip>(newBgmName))
                return;
        }
        
        _bgmTweener.ChangeStartValue(GlobalBgmAudio.volume, 0.5f);
        _bgmTweener.ChangeEndValue(0.0f, 0.5f);
        _bgmTweener.OnComplete(() =>
        {
            GlobalBgmAudio.clip = AssetManager.LoadAsset<AudioClip>(newBgmName);
            GlobalBgmAudio.loop = true;
            GlobalBgmAudio.Play();
            _bgmTweener.ChangeStartValue(GlobalBgmAudio.volume, 0.5f);
            _bgmTweener.ChangeEndValue(1.0f, 0.5f);
            _bgmTweener.OnComplete(() => { }).Play();

        }).Play();
    }

    #region Controller Fixed Update执行，以及Mini Core的Command队列执行
    void FixedUpdate()
    {
        MiniCore.Execute();
        foreach (var controller in MiniCore.Controllers.Values)
        {
            if (controller.EnableFixedUpdate)
            {
                controller.FixedUpdate();
            }
        }
        
    }
    #endregion

    #region Debug Dialog 按键检测

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
#if UNITY_EDITOR
            DebugView debug = ViewManager.GetView<DebugView,DebugCanvas>(false);
            if (debug != null)
                debug.gameObject.SetActive(!debug.gameObject.activeInHierarchy);
            else
            {
                debug = ViewManager.GetView<DebugView, DebugCanvas>(true);
            }
#endif
        }
    }

    #endregion
}
