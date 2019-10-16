using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class BookEnemy : EnemyObject
{

    public override void Init()
    {
        base.Init();
        CurrentState = ObjectState.Flying;
    }

    public override void OnCollideWithBody(MainCharacter character, Collision2D col)
    {
        base.OnCollideWithBody(character, col);
        switch (CurrentState)
        {
            case ObjectState.Flying:
                MiniCore.Get<CharacterController>().CurrentHealth -= 1;
                CurrentState = ObjectState.Obsorbed;
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
            {
                if (hand.handType == HandType.RejectHand)
                {
                    CurrentState = ObjectState.Rejecting;
                    OnRejectWithHandEffect(hand, col);
                    MiniCore.Get<CharacterController>().Score += 5;
                    CaculateVelocity(hand);
                    this.Get<CharacterController>().AbilityProgress += 10;

                    if (hand.typechange)
                    {

                        CurrentState = ObjectState.Obsorbed;

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
                    if (hand.handType == HandType.AbsorbHand)
                    {
                        CurrentState = ObjectState.Obsorbed;
                        MiniCore.Get<CharacterController>().CurrentHealth -= 1;
                    }
                }

                break;
            }

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

        if (CurrentState == ObjectState.Rejecting || CurrentState == ObjectState.Flying)
        {
            if (enemy.GetType() == typeof(PaperBallEnemy) || enemy.GetType() == typeof(ChalkEnemy))
            {
                enemy.ObsorbedByMagicField(false);
            }

            if (enemy.GetType() == typeof(PencilEnemy))
            {
                (enemy as PencilEnemy).StopRotate();
                enemy.ObsorbedByMagicField(false);
            }
        }
    }

    //
    public override void ExitCollideWithEnemy(EnemyObject enemy, Collision2D col)
    {
        base.ExitCollideWithEnemy(enemy, col);

        if (CurrentState == ObjectState.Flying)
        {
            Vector2 dir = transform.position;
            dir.Normalize();
            CachedRigidbody.velocity = -dir * Random.Range(2f, 3f);
            CachedRigidbody.angularVelocity = Random.Range(90f, 360f);
        }
        else
        {
        }

    }
}
