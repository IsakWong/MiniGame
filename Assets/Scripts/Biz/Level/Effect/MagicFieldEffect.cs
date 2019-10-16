using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MagicFieldEffect : ManagedObject
{
    public float LifeTime = 4;
    private float _passedLife = 0;

    public SpriteRenderer magicSprite1;
    public SpriteRenderer magicSprite2;
    public SpriteRenderer magicSprite3;
    public Light pointLight;

    private Sequence seq;

    protected void Awake()
    {
        base.Awake();
        seq = DOTween.Sequence().SetAutoKill(false);
        seq.Insert(0.1f, magicSprite1.DOFade(1.0f, 0.5f));
        seq.Insert(0.2f, magicSprite2.DOFade(1.0f, 0.5f));
        seq.Insert(0.3f, magicSprite3.DOFade(1.0f, 0.5f));
        seq.Insert(0.1f, magicSprite1.transform.DOScale(1.5f * Vector3.one, 0.3f).SetEase(Ease.OutBack));
        seq.Insert(0.2f, magicSprite2.transform.DOScale(1.5f *Vector3.one, 0.4f).SetEase(Ease.OutBack));
        seq.Insert(0.3f, magicSprite3.transform.DOScale(1.5f *Vector3.one, 0.5f).SetEase(Ease.OutBack));
        seq.Insert(0.2f, DOTween.To(() => pointLight.range, x => pointLight.range = x, 15, 0.5f));
        seq.Insert(0.1f, magicSprite1.transform.DORotate(new Vector3(0,0,180), 0.5f,RotateMode.WorldAxisAdd));
        seq.Insert(0.2f, magicSprite2.transform.DORotate(new Vector3(0, 0, -180), 0.5f, RotateMode.WorldAxisAdd));
        seq.Insert(0.3f, magicSprite3.transform.DORotate(new Vector3(0, 0, -180), 0.5f, RotateMode.WorldAxisAdd));
        seq.AppendInterval(1.25f).SetLoops(2,LoopType.Yoyo).Pause();
    }

    protected void OnEnable()
    {
        seq.Restart();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        _passedLife += Time.fixedDeltaTime;
        if (_passedLife > LifeTime)
        {
            _passedLife = 0;
            Recycle();
        }
    }
}
