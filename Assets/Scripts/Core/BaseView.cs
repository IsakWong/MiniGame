using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class BaseView : MonoBehaviour
{
    public string ViewName;
    protected void Awake()
    {
        _cachedCanvasGroup = GetComponent<CanvasGroup>();
        if (ViewManager.Instance.Views.TryGetValue(GetType().ToString(), out BaseView view))
        {

        }
        else
        {
            ViewManager.Instance.Views.Add(GetType().ToString(),this);
        }
    }

    // Start is called before the first frame update
    protected void Start()
    {
        
    }

    protected void FixedUpdate()
    {

    }


    protected void OnDestroy()
    {
        ViewManager.RemoveView<BaseView>(GetType().Name);
    }

    #region 公共接口
    public void CloseSelf()
    {
        Destroy(this.gameObject);
    }
    #endregion

    public virtual void OnPlayInAnimation()
    {

    }

    public virtual void OnPlayOutAnimation()
    {

    }
    #region Animation


    public enum InAnimationType
    {
        FadeIn,
        SlipIn,
        BoomIn,
    }

    public enum InSlipType
    {
        FromLeft,
        FromRight,
        FromTop,
        FromBottom
    }
    public enum OutSlipType
    {
        ToLeft,
        ToRight,
        ToTop,
        ToBottom
    }
    public enum OutAnimationType
    {
        FadeOut,
        SlipOut,
        BoomOut
    }

    [Header("Animation Settings")]
    public InAnimationType InType = InAnimationType.FadeIn;
    public OutAnimationType OutType = OutAnimationType.FadeOut;

    public InSlipType ViewInSlipType = InSlipType.FromLeft;
    public OutSlipType ViewOutSlipType = OutSlipType.ToLeft;

    public float InTime = 0.3f;
    public float OutTime = 0.3f;

    private Action inOverCallback = null;
    private Action outOverCallback = null;
    private CanvasGroup _cachedCanvasGroup;
    // Start is called before the first frame update


    public void FadeUpdate(float value)
    {
        if (_cachedCanvasGroup != null)
        {
            _cachedCanvasGroup.alpha = value;
        }

    }

    public void InOverCallback()
    {
        if (inOverCallback != null)
            inOverCallback.Invoke();

    }
    public void OutOverCallback()
    {
        if (outOverCallback != null)
            outOverCallback.Invoke();

    }
    public void PlayInAnim(Action AnimOverCallback = null)
    {
        if (_cachedCanvasGroup != null)
            _cachedCanvasGroup.blocksRaycasts = false;
        RectTransform rt = _cachedCanvasGroup.GetComponent<RectTransform>();
        OnPlayInAnimation();
        switch (InType)
        {
            case InAnimationType.FadeIn:
                _cachedCanvasGroup.alpha = 0.0f;
                _cachedCanvasGroup.DOFade(1f, InTime).SetEase(Ease.OutExpo).OnComplete(delegate ()
                {
                    if (_cachedCanvasGroup != null)
                        _cachedCanvasGroup.blocksRaycasts = true;
                    if (AnimOverCallback != null)
                        AnimOverCallback.Invoke();
                });
                break;
            case InAnimationType.SlipIn:
                Vector2 size = rt.rect.size;
                rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, -size.x, size.x);
                rt.DOAnchorPos3DX(rt.anchoredPosition3D.x + size.x, InTime).SetEase(Ease.OutExpo).OnComplete(delegate ()
                {
                    if (_cachedCanvasGroup != null)
                        _cachedCanvasGroup.blocksRaycasts = true;
                    if (AnimOverCallback != null)
                        AnimOverCallback.Invoke();
                });
                break;
            case InAnimationType.BoomIn:
                rt.localScale = Vector3.zero;
                rt.DOScale(Vector3.one, InTime).SetEase(Ease.OutExpo).OnComplete(delegate ()
                {
                    if (_cachedCanvasGroup != null)
                        _cachedCanvasGroup.blocksRaycasts = true;
                    if (AnimOverCallback != null)
                        AnimOverCallback.Invoke();
                });
                break;
        }
    }

    public void PlayOutAnim(Action AnimOverCallback = null)
    {
        if (_cachedCanvasGroup != null)
            _cachedCanvasGroup.blocksRaycasts = false;

        RectTransform rt = _cachedCanvasGroup.GetComponent<RectTransform>();
        OnPlayOutAnimation();
        switch (OutType)
        {
            case OutAnimationType.FadeOut:
                _cachedCanvasGroup.DOFade(0.0f, OutTime).SetEase(Ease.InExpo).OnComplete(delegate ()
                {
                    if (AnimOverCallback != null)
                        AnimOverCallback.Invoke();
                });
                break;
            case OutAnimationType.SlipOut:
                Vector2 size = rt.rect.size;
                rt.DOAnchorPos3DX(rt.anchoredPosition3D.x - size.x, InTime).SetEase(Ease.InExpo);
                break;
            case OutAnimationType.BoomOut:
                rt.DOScale(Vector3.zero, OutTime).SetEase(Ease.InBack).OnComplete(delegate ()
                {
                    if (_cachedCanvasGroup != null)
                        _cachedCanvasGroup.blocksRaycasts = true;
                    if (AnimOverCallback != null)
                        AnimOverCallback.Invoke();
                });
                break;
        }
    }

    #endregion

}
