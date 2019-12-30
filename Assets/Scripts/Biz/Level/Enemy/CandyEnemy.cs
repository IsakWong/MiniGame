using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mini.Core;
using UnityEngine;
using Random = UnityEngine.Random;

public class CandyEnemy : EnemyObject
{

    public override void Init()
    {
        base.Init();
        CurrentState = ObjectState.Flying;
        CachedRigidbody.angularVelocity = Random.Range(0f, 180f);
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

                if (hand.handType == HandType.RejectHand)
                {
                    CurrentState = ObjectState.Rejecting;
                    CaculateVelocity(hand);
                    OnRejectWithHandEffect(hand, col);

                    if (hand.typechange)
                    {
                        ObjectManager.CreateManagedObject("SmokeBoom").transform.position = transform.position;
                        ViewManager.GetView<LevelOverlayView>().ShowTip("糖果可以回复1点生命值。", 2.5f);
                        MiniCore.PlaySound("获得糖果音效");
                        CurrentState = ObjectState.Obsorbed;
                        if (MiniCore.Get<CharacterController>().CurrentHealth < 3)
                        {
                            MiniCore.Get<CharacterController>().CurrentHealth++;
                        }


                        if (hand.enemyobject.Enemyhealth == 0)
                        {
                            hand.handType = HandType.AbsorbHand;
                            hand.typechange = false;
                            hand.enemyobject.CurrentState = ObjectState.Obsorbed;
                        }
                        else
                        {
                            hand.enemyobject.Enemyhealth--;
                        }
                    }

                }
                else
                {
                    ObjectManager.CreateManagedObject("SmokeBoom").transform.position = transform.position;
                    ViewManager.GetView<LevelOverlayView>().ShowTip("糖果可以回复1点生命值。", 2.5f);
                    MiniCore.PlaySound("获得糖果音效");
                    CurrentState = ObjectState.Obsorbed;
                    if (MiniCore.Get<CharacterController>().CurrentHealth < 3)
                    {
                        MiniCore.Get<CharacterController>().CurrentHealth++;
                    }

                }

                break;
            case ObjectState.Rejecting:
                break;
            case ObjectState.Obsorbed:
                break;
            case ObjectState.Destroyed:
                break;
        }
    }

    public override void OnCollideWithEnemy(EnemyObject enemy, Collision2D col)
    {
        base.OnCollideWithEnemy(enemy, col);
        ////GameObject star = null;
        //if (CurrentState == ObjectState.Rejecting || CurrentState == ObjectState.Flying)
        //{
        //    if (enemy.GetType() == typeof(PaperBallEnemy) || enemy.GetType() == typeof(ChalkEnemy))
        //    {
        //        enemy.CurrentState = ObjectState.Obsorbed;

        //        //todo: change the property of the character
        //    }

        //    if (enemy.GetType() == typeof(PencilEnemy))
        //    {
        //        enemy.CurrentState = ObjectState.Rejecting;
        //        Vector2 dir = enemy.transform.position - transform.position;
        //        dir.Normalize();
        //        enemy.CachedRigidbody.velocity = dir * Random.Range(2f, 3f);
        //        enemy.CachedRigidbody.angularVelocity = Random.Range(90f, 360f);
        //    }
        //}
    }

}
