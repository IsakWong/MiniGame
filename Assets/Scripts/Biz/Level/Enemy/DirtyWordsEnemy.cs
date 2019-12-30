using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mini.Core;
using UnityEngine;
using Random = UnityEngine.Random;

public class DirtyWordsEnemy : EnemyObject
{
    public Sprite[] texuters;

    public override void Init()
    {
        base.Init();
        CurrentState = ObjectState.Flying;
        GetComponent<SpriteRenderer>().sprite = texuters[Random.Range(0, texuters.Length)];
    }



    public override void OnCollideWithBody(MainCharacter character, Collision2D col)
    {
        base.OnCollideWithBody(character, col);
        if(CurrentState == ObjectState.Flying)
            MiniCore.Get<CharacterController>().CurrentHealth--;

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
                    OnRejectWithHandEffect(hand, col);
                }

                if (hand.handType == HandType.AbsorbHand)
                {
                    CurrentState = ObjectState.Rejecting;
                    CaculateVelocity(hand);
                    MiniCore.Get<CharacterController>().CurrentHealth--;
                }
                break;

        }
        CaculateVelocity(hand);
    }

    public override void OnCollideWithEnemy(EnemyObject enemy, Collision2D col)
    {
        base.OnCollideWithEnemy(enemy, col);
    }
}
