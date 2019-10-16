using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

using Random = UnityEngine.Random;


public class LoveLetterEnemy : EnemyObject
{
    public Vector3 origPos = Vector3.zero;

    public override void Init()
    {
        base.Init();
        CurrentState = ObjectState.Flying;
        origPos = transform.position;
    }



    public override void OnCollideWithBody(MainCharacter character, Collision2D col)
    {
        base.OnCollideWithBody(character, col);
        switch (CurrentState)
        {
            case ObjectState.Flying:

                CurrentState = ObjectState.Rejecting;
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

        switch (CurrentState) {
            case ObjectState.Flying:

                if (hand.handType == HandType.AbsorbHand)
                {
                    CurrentState = ObjectState.Obsorbed;
                    hand.MainCharacter.collideWithLoveLetter = true;
                    hand.MainCharacter.heartStartPos = origPos;
                }
                else {
                    CurrentState = ObjectState.Rejecting;
                    CaculateVelocity(hand);
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
}
