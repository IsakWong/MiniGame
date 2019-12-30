using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mini.Core;
using UnityEngine;
using Random = UnityEngine.Random;

public class StarEnemy : EnemyObject
{

    private Sequence starSequnce;
    private Sequence lifeSeq;
    public override void Init()
    {
        base.Init();
        CurrentState = ObjectState.Flying;
        transform.SetParent(null);
        var pos = MiniCore.Get<GameController>().CurrentWorld.Main.transform.position - transform.position;
        pos.z = 0;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, pos.normalized);
    }

    public override void OnPause()
    {
        base.OnPause();
        if (lifeSeq != null && lifeSeq.IsActive())
        {
            lifeSeq.timeScale = 0;
        }
        if (starSequnce != null && starSequnce.IsActive())
        {
            starSequnce.timeScale = 0;
        }
    }

    public override void OnRestore()
    {
        base.OnRestore();
        if (lifeSeq != null && lifeSeq.IsActive())
        {
            lifeSeq.timeScale = 1.0f;
        }
        if (starSequnce != null && starSequnce.IsActive())
        {
            starSequnce.timeScale = 1.0f;
        }
    }

    protected void Awake()
    {
        base.Awake();
        starSequnce = DOTween.Sequence();
        starSequnce.AppendCallback(CreateStartEffect);
        starSequnce.AppendInterval(0.2f);
        starSequnce.SetLoops(25).Pause().SetAutoKill(false);
        lifeSeq = DOTween.Sequence();
        lifeSeq.InsertCallback(5.0f, delegate ()
        {
            DestroyByMagicField();
        }).SetAutoKill(false).Pause();
    }

    private void CreateStartEffect()
    {

        GameObject starProjectile = ObjectManager.CreateManagedObject("YellowProjectile");
        Vector3 dir = transform.position - MiniCore.Get<GameController>().CurrentWorld.Main.transform.position;
        dir.Normalize();
        starProjectile.transform.position = transform.position;
        starProjectile.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
    }


    public override void OnCollideWithBody(MainCharacter character, Collision2D col)
    {
        base.OnCollideWithBody(character, col);
        switch (CurrentState)
        {
            case ObjectState.Flying:

                CurrentState = ObjectState.Rejecting;
                Vector2 dir = transform.position - character.transform.position;
                dir.Normalize();
                CachedRigidbody.velocity = dir * Random.Range(2f, 3f);
                CachedRigidbody.angularVelocity = Random.Range(90f, 360f);
                break;

            case ObjectState.Rejecting:
                break;
            case ObjectState.Obsorbed:
                break;
            case ObjectState.Destroyed:
                break;
        }

    }

    public override void OnCollideWithHand(Hand hand, Collider2D col)
    {
        base.OnCollideWithHand(hand, col);

        switch (CurrentState)
        {
            case ObjectState.Flying:

                if (hand.handType == HandType.AbsorbHand)
                {
                    ViewManager.GetView<LevelOverlayView>().ShowTip("用手中的光去消灭周围的危险恐惧。", 2.5f);
                    MiniCore.PlaySound("捡起星星");
                    CachedRigidbody.velocity = Vector2.zero;
                    CachedRigidbody.angularVelocity = 0.0f;
                    CachedRigidbody.simulated = false;

                    gameObject.transform.SetParent(hand.GrabPoint.transform);
                    gameObject.transform.localPosition = Vector3.zero;

                    starSequnce.timeScale = MiniCore.TimeScale;
                    
                    lifeSeq.timeScale = MiniCore.TimeScale;
                    starSequnce.Restart();
                    lifeSeq.Restart();

                }
                else
                {
                    CurrentState = ObjectState.Rejecting;

                    Vector2 dir = transform.position - hand.transform.position;
                    dir.Normalize();
                    CachedRigidbody.velocity = dir * Random.Range(2f, 3f);
                    CachedRigidbody.angularVelocity = Random.Range(90f, 360f);

                    if (hand.typechange)
                    {
                        MainCharacter curCharacter = hand.MainCharacter;
                        curCharacter.collideWithStar = true;
                        CurrentState = ObjectState.Obsorbed;

                        hand.enemyobject.CurrentState = ObjectState.Obsorbed;
                        hand.enemyobject.CachedRigidbody.simulated = true;
                        hand.enemyobject.transform.SetParent(null);

                        hand.typechange = false;
                        hand.handType = HandType.RejectHand;

                        hand.enemyobject = this;

                    }
                }
                break;
            case ObjectState.Obsorbed:
                break;
            case ObjectState.Destroyed:
                break;
            case ObjectState.Rejecting:
                break;

        }
    }

    public override void OnCollideWithEnemy(EnemyObject enemy, Collision2D col)
    {
        base.OnCollideWithEnemy(enemy, col);

    }

    public override void ExitCollideWithHand(Hand hand, Collision2D col)
    {
        base.ExitCollideWithHand(hand, col);
        if (hand.handType == HandType.RejectHand)
        {
            Vector2 dir = transform.position - hand.transform.position;
            dir.Normalize();
            CachedRigidbody.velocity = dir * Random.Range(2f, 3f);
            CachedRigidbody.angularVelocity = Random.Range(90f, 360f);
        }
    }

    protected new void OnRecycle()
    {
        base.OnRecycle();
        transform.SetParent(null);
    }

}
