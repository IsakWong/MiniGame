using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mini.Core;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlackCharacter : EnemyObject
{
    public int health = 8;

    public Light AbsorbHandLight;
    public Light RejectHandLight;
    public PolygonCollider2D AbsorbHandCollider;

    private Tweener rotationTweener;
    private Tweener moveTweener;
    private Tweener hitTweener;

    bool _isdied = false;
    public static int winCount = 0;
    WorldLevel5 WorldLevel5;
    public Material Material1;
    Material material;

    public Material Material2;
    Material material2;
    public SpriteRenderer mask;
    public override void Init()
    {
        base.Init();
        Vector3 target = MiniCore.Get<GameController>().CurrentWorld.Main.transform.position;
        Vector3 dir = target - transform.position;
        transform.rotation = Quaternion.FromToRotation(Vector3.right, dir.normalized);
        moveTweener = transform.DOMove(target, 20);
        rotationTweener.Restart();
        PrePareEverything();
    }

    public override void OnPause()
    {
        base.OnPause();
        moveTweener.timeScale = 0;
        rotationTweener.timeScale = 0;
    }

    public override void OnRestore()
    {
        base.OnRestore();
        moveTweener.timeScale = 1;
        rotationTweener.timeScale = 1;
    }

    protected void OnDisable()
    {
        base.OnDisable();
        moveTweener.Kill(false);
    }
    public void Win()
    {
        this.Get<GameController>().Win();
    }
    public void TakeHit()
    {
        hitTweener.Restart();
        health--;
        if (health <= 0)
        {
            _isdied = true;
            Absorbed();
            //召唤弹幕
            Fire(false);
          
        } else

        {
            MoveBackward();
        }


    }

    public void TakeHitBad()
    {
        MiniCore.Get<CharacterController>().CurrentHealth--;
        MoveBackward();
    }

    public override void OnCollideWithBody(MainCharacter character, Collision2D col)
    {
        base.OnCollideWithBody(character, col);
        TakeHitBad();
    }

    public bool HitBossAbsoredArm(Hand hand, Collider2D col)
    {
        if (AbsorbHandCollider.IsTouching(col))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public override void OnCollideWithHand(Hand hand, Collider2D col)
    {
        base.OnCollideWithHand(hand, col);

    }
    void OnTriggerEnter2D(Collider2D col)
    {
        Hand curHand = col.gameObject.GetComponent<Hand>();
        if (curHand != null)
        {
            if (curHand.handType == HandType.RejectHand)
            {
                TakeHit();
                OnRejectWithHandEffect(curHand, col);
            }
            else
            {
                if (HitBossAbsoredArm(curHand, col))
                {
                    _isdied = true;
                    Absorbed();
                    Fire(true);
                }
                else
                {
                    TakeHitBad();
                }
            }
        }
        if (winCount >= 2)
        {
            ViewManager.GetView<LevelOverlayView>().ShowBossTip2();
            Invoke("Win", 2f);
        }
    }

    public override void OnCollideWithEnemy(EnemyObject enemy, Collision2D col)
    {
        base.OnCollideWithEnemy(enemy, col);
    }

    private void Awake()
    {
        base.Awake();
        Vector3 target = MiniCore.Get<GameController>().CurrentWorld.Main.transform.position;
        Vector3 dir = target - transform.position;
        rotationTweener = transform.DORotate(new Vector3(0, 0, 360), 1, RotateMode.WorldAxisAdd).SetLoops(-1, LoopType.Incremental).SetAutoKill(false);
        PrePareEverything();

    }
    private void OnEnable()
    {
        base.OnEnable();
        PrePareEverything();
        hitTweener = this.GetComponent<SpriteRenderer>().DOColor(Color.red, 0.1f).SetLoops(2, LoopType.Yoyo).SetAutoKill(false).Pause();
        
    }
    public void Reset()
    {
        PrePareEverything();
    }
    public void Fire(bool _isAbsored)
    {
        moveTweener.Pause();
        rotationTweener.Pause();
        if (_isAbsored)
        {
            material2.DOFloat(0.75f, "_Progress", 1.0f).OnComplete(delegate ()
            {
                mask.DOFade(0, 1f);
                this.GetComponent<SpriteRenderer>().DOFade(0, 1f).OnComplete(delegate ()
                {
                    this.gameObject.SetActive(false);
                });
            }
            );

        }
        else
        {
            material.SetFloat("_Progress", 0.75f);
            material.DOFloat(0, "_Progress", 3.0f).OnComplete(delegate ()
            {
                this.gameObject.SetActive(false);
            });
        }


    }
    void MoveBackward()
    {
        Vector3 target = MiniCore.Get<GameController>().CurrentWorld.Main.transform.position;
        Vector3 dir = (transform.position - target).normalized;
        moveTweener.ChangeStartValue(transform.position);

        moveTweener.ChangeEndValue(transform.position + dir * 2f, 1f).OnComplete(delegate ()
        {
            moveTweener.ChangeStartValue(transform.position);
            moveTweener.ChangeEndValue(target, 5);
            moveTweener.Restart();
        });
        moveTweener.Restart();
    }
    void Absorbed()
    {
        Collider2D[] collider2Ds = this.GetComponents<Collider2D>(); 
        foreach (Collider2D collider2D in collider2Ds)
        {
            collider2D.enabled = false;
        }
        winCount++;
    }
    void PrePareEverything()
    {
        WorldLevel5 = GameObject.FindGameObjectWithTag("WorldLevel").GetComponent<WorldLevel5>();
        Collider2D[] collider2Ds = this.GetComponents<Collider2D>();
        foreach (Collider2D collider2D in collider2Ds)
        {
            collider2D.enabled = true;
        }
        material = new Material(Material1);
        material.SetFloat("_Progress", 1.0f);
        this.GetComponent<SpriteRenderer>().material = material;
        material2 = new Material(Material2);
        material2.SetFloat("_Progress", 0f);
        mask.material = material2;

        health = 8;

    }
    private void FixedUpdate()
    {
        if (health <= 2)
        {
            this.GetComponent<SpriteRenderer>().color = new Color(1, 0.6f, 0.6f);
            if (health <= 1)
            {
                this.GetComponent<SpriteRenderer>().color = Color.red;
            }
        }
        else
        {
            this.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}
