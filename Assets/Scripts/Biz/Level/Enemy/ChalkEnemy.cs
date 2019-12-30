using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mini.Core;
using UnityEngine;
using Random = UnityEngine.Random;

public class ChalkEnemy : EnemyObject
{
    public Sprite[] textures;
    public override void Init()
    {
        base.Init();
        CurrentState = ObjectState.Flying;
        GetComponent<SpriteRenderer>().sprite = textures[Random.Range(0, textures.Length)];
        var pos = MiniCore.Get<GameController>().CurrentWorld.Main.transform.position - transform.position;
        pos.z = 0;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, pos.normalized);
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

                if (hand.handType == HandType.RejectHand)
                {
                    CurrentState = ObjectState.Rejecting;
                    MiniCore.Get<CharacterController>().Score += 5;
                    OnRejectWithHandEffect(hand, col);
                    this.Get<CharacterController>().AbilityProgress += 10;
                    CaculateVelocity(hand);
                    if (hand.typechange)
                    {

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
                    CurrentState = ObjectState.Obsorbed;
                    MiniCore.Get<CharacterController>().CurrentHealth--;
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
    }
    
}
