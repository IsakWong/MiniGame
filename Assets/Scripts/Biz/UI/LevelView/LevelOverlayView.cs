using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LevelOverlayView : BaseView
{
    public GameObject HeartPrefab;

    public HorizontalLayoutGroup HeartGroup;


    public RectTransform ComboPanel;
    public Text ComboText;

    public RectTransform TopPanel;
    public RectTransform TipPanel;
    public RawImage Red;
    public RawImage Black;
    public Button PauseBtn;
    public Text ScoreText;
    public Text TipText;
    public Text TimeText;

    public RawImage BossTip1;
    public RawImage BossTip2;

    private Tweener tweener;
    private Sequence comboSeq;
    private Sequence comboTextSeq;
    private float Target = 1.5f;

    private Sequence tipSeq;

    // Start is called before the first frame update
    public override void OnPlayInAnimation()
    {
        base.OnPlayInAnimation();
        TopPanel.DOAnchorPosY(0, 1.0f);
    }

    public override void OnPlayOutAnimation()
    {
        base.OnPlayOutAnimation();
        TopPanel.DOAnchorPosY(360, 1.0f).SetEase(Ease.InBack);
    }

    public void ShowBossTip1()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(BossTip1.DOColor(Color.white, 0.5f));
        seq.AppendInterval(2f);
        seq.Append(BossTip1.DOColor(new Color(1,1,1,0), 0.5f));
    }
    public void ShowBossTip2()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(BossTip2.DOColor(Color.white, 0.5f));
        seq.AppendInterval(2f);
        seq.Append(BossTip2.DOColor(new Color(1, 1, 1, 0), 0.5f));
    }
    public void ShowTip(string tipText, float duration)
    {
        if (tipSeq == null)
            tipSeq = DOTween.Sequence().Pause().SetAutoKill(false);
        else
            return;
        TipText.text = tipText;
        tipSeq.Append(TipPanel.DOAnchorPosY(-16, 0.5f).SetEase(Ease.OutBack));
        tipSeq.AppendInterval(duration);
        tipSeq.Append(TipPanel.DOAnchorPosY(192, 0.5f));
        tipSeq.AppendCallback(delegate ()
        {
            tipSeq = null;
        });
        tipSeq.Restart();
        comboSeq = DOTween.Sequence();
        comboSeq.Append(ComboPanel.DOAnchorPosX(32, 0.2f).SetEase(Ease.OutBack)).SetEase(Ease.OutBack);
        comboSeq.AppendInterval(0.5f);
        comboSeq.Append(ComboPanel.DOAnchorPosX(-400, 0.2f).SetEase(Ease.OutBack)).Play();
    }
    public void DoRedFade()
    {
        tweener.Restart();
    }


    protected new void Awake()
    {
        base.Awake();

        PauseBtn.onClick.AddListener(delegate ()
        {
            Black.DOColor(new Color(0, 0, 0, 0.3f), 0.5f);
            (HeartGroup.transform as RectTransform).DOAnchorPosY(-170, 0.5f);
            PauseBtn.interactable = false;
            RectTransform rect = PauseBtn.transform as RectTransform;
            this.Get<GameController>().Pause();

        });

        tweener = Red.DOColor(new Color(1, 0, 0, 0.2f), 0.1f).SetLoops(2, LoopType.Yoyo).SetAutoKill(false).Pause();
        this.AddListener<int,int>(GlobalGameMessage.OnComboChange,OnComboChange);
        this.AddListener<int, int>(GlobalGameMessage.OnHealthChange, OnHealthChange);

        OnHealthChange(0, this.Get<CharacterController>().CurrentHealth);

    }

    private void OnComboChange(int old, int value)
    {
        ComboText.text = "<size=60>Combo X</size>" + value.ToString();
        float scale = Mathf.Clamp(1f + value / 10f, 1f, 2.0f);
        if (value == 0)
        {
            if (comboSeq == null)
            {
                comboSeq = DOTween.Sequence();
                comboSeq.Append(ComboPanel.DOAnchorPosX(-400, 0.4f)).Play();
            }
            else
            {
                if (comboSeq.IsActive())
                {
                    comboSeq.Kill(false);
                }
                comboSeq = DOTween.Sequence();
                comboSeq.Append(ComboPanel.DOAnchorPosX(-400, 0.4f)).Play();
            }
        }
        else
        {
            if (comboSeq == null)
            {
                comboSeq = DOTween.Sequence();
                comboSeq.Append(ComboPanel.DOAnchorPosX(32, 0.2f).SetEase(Ease.OutBack)).SetEase(Ease.OutBack);
                comboSeq.AppendInterval(2.0f);
                comboSeq.Append(ComboPanel.DOAnchorPosX(-400, 0.4f).SetEase(Ease.OutBack)).Play();
            }
            else
            {
                if (comboSeq.IsActive())
                {
                    comboSeq.Kill(false);
                }
                comboSeq = DOTween.Sequence();
                comboSeq.Append(ComboPanel.DOAnchorPosX(32, 0.2f));
                comboSeq.AppendInterval(2.0f);
                comboSeq.Append(ComboPanel.DOAnchorPosX(-400, 0.4f)).Play();

            }
            if (comboTextSeq == null)
            {
                comboTextSeq = DOTween.Sequence();
                comboTextSeq.Append(ComboText.transform.DOScale(new Vector3(scale,scale,scale), 0.3f)).SetEase(Ease.OutBack);
                comboTextSeq.AppendInterval(0.1f);
                comboTextSeq.Append(ComboText.transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.3f)).Play();
            }
            else
            {
                if (comboTextSeq.IsActive())
                {
                    comboTextSeq.Kill(false);
                }
                comboTextSeq = DOTween.Sequence();
                comboTextSeq.Append(ComboText.transform.DOScale(new Vector3(scale, scale, scale), 0.3f)).SetEase(Ease.OutBack);
                comboTextSeq.AppendInterval(0.1f);
                comboTextSeq.Append(ComboText.transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.3f)).Play();
            }
        }



    }

    private Tweener scoreTween;
    private Tweener smallTween;

    private float _rate;

    public void SetHeartEnable(bool value)
    {
        HeartGroup.gameObject.SetActive(value);
    }
    public void SetScoreEnable(bool value)
    {
        ScoreText.gameObject.SetActive(value);
    }


    public void OnHealthChange(int oldValue, int newValue)
    {
        if (HeartGroup.transform.childCount < newValue)
        {
            int Size = HeartGroup.transform.childCount;
            for (int i = 0; i < Size; ++i)
            {
                GameObject.Instantiate(HeartPrefab, HeartGroup.transform);
                HeartGroup.transform.GetChild(i).localScale = Vector3.one;
                HeartGroup.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < newValue; ++i)
            {
                HeartGroup.transform.GetChild(i).gameObject.SetActive(true);
                HeartGroup.transform.GetChild(i).localScale = Vector3.one;
           
            }
        }
    }
    protected void OnDestroy()
    {
        base.OnDestroy();
        this.RemoveListener<int, int>(GlobalGameMessage.OnHealthChange, OnHealthChange);
        this.RemoveListener<int, int>(GlobalGameMessage.OnComboChange, OnComboChange);
    }

    // Update is called once per frame
    protected new void FixedUpdate()
    {
        //_InternalScale();
        _rate += Time.fixedDeltaTime;
        if (_rate > 1.0f)
        {
            _rate = 0.0f;
            var controller = this.Get<GameController>();
            if (controller.CurrentWorld != null)
            {
                float s = (controller.CurrentWorld.timeSinceLevelBegin / controller.CurrentWorld.WinTime);
                TimeText.text = String.Format("{0:D2}:{1:D2}", this.Get<GameController>().CurrentWorld.Hour,
                    (int) (s * 60f));
            }
            
        }
    }
}
