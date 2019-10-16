using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class HandEnemy : EnemyObject
{
    private SpriteRenderer spriteRenderer;
    private Tweener hitTweener;
    private Tweener moveTweener;

    private float curTime = 0.0f;

    public bool tmp = false;
    public override void Init()
    {
        base.Init();

        gameObject.layer = LayerMask.NameToLayer("Enemy");
        tmp = false;
        Vector3 target = MiniCore.Get<GameController>().CurrentWorld.Main.transform.position;
        Vector3 dir = target - transform.position;
        transform.rotation = Quaternion.FromToRotation(Vector3.right, dir.normalized);
        moveTweener = transform.DOMove(target, 20);
    }
    public override void OnPause()
    {
        base.OnPause();
        if (hitTweener != null)
            hitTweener.timeScale = 0;
        if (moveTweener != null)
            moveTweener.timeScale = 0;
    }

    public override void OnRestore()
    {
        base.OnRestore();
        if (hitTweener != null)
            hitTweener.timeScale = 1f;
        if (moveTweener != null)
            moveTweener.timeScale = 1f;
    }
    protected void OnDisable()
    {
        base.OnDisable();
        moveTweener.Kill(false);
    }
    public void TakeHit()
    {
        Vector3 target = MiniCore.Get<GameController>().CurrentWorld.Main.transform.position;
        Vector3 dir = (transform.position - target).normalized;
        moveTweener.ChangeStartValue(transform.position);
        moveTweener.ChangeEndValue(transform.position + dir * 3f, 1f).OnComplete(delegate ()
        {
            moveTweener.ChangeStartValue(transform.position);
            moveTweener.ChangeEndValue(target, 20);
            moveTweener.Restart();
        });
        moveTweener.Restart();
        hitTweener.Restart();
    }
    protected void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (hitTweener == null)
            hitTweener = spriteRenderer.DOColor(Color.red, 0.1f).SetLoops(2, LoopType.Yoyo).SetAutoKill(false).Pause();

    }
    public override void OnCollideWithBody(MainCharacter character, Collision2D col)
    {
        base.OnCollideWithBody(character, col);
    }

    public override void OnCollideWithHand(Hand hand, Collider2D col)
    {
        base.OnCollideWithHand(hand, col);
        if (tmp)
        {
            return;
        }
        if(hand.handType == HandType.RejectHand)
        {
            TakeHit();
        }
        else
        {
            MainCharacter main = hand.MainCharacter;
            moveTweener.Pause();
            if (main != null)
            {
                main.RotateSpeed -= 20;
                if (main.RotateSpeed < 0)
                {
                    main.RotateSpeed = 10;
                }
            }
        }
      
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        MainCharacter main = col.gameObject.GetComponent<MainCharacter>();
        if (main != null)
        {
            Debug.Log("碰到黑手");
            main.RotateSpeed -= 20;
            if (main.RotateSpeed < 0)
            {
                main.RotateSpeed = 10;
            }
            moveTweener.Pause();

        }

    }

    void OnTriggerExit2D(Collider2D col)
    {
        MainCharacter main = col.gameObject.GetComponent<MainCharacter>();
        if (main != null)
        {
            moveTweener.Pause();
            main.RotateSpeed += 20;
            if (main.RotateSpeed > 30)
            {
                main.RotateSpeed = 30;
            }
        }
        
    }


    public override void OnCollideWithEnemy(EnemyObject enemy, Collision2D col)
    {
        base.OnCollideWithEnemy(enemy, col);
    }

    bool firstCondition = true;
    
    public void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
