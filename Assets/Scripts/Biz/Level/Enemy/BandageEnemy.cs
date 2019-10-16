using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class BandageEnemy : EnemyObject
{

    public override void Init()
    {
        base.Init();
        CurrentState = ObjectState.Flying;
        CachedRigidbody.angularVelocity = Random.Range(90f, 270f);
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
        StarBloom star = null;
        switch (CurrentState)
        {
            case ObjectState.Flying:
                if (hand.handType == HandType.RejectHand)
                {
                    OnRejectWithHandEffect(hand, col);
                    CurrentState = ObjectState.Rejecting;
                    MiniCore.Get<CharacterController>().Score += 5;
                    this.Get<CharacterController>().AbilityProgress += 10;
                    CaculateVelocity(hand);
                    
                    if (hand.typechange) {
                        Debug.Log("Enemyhealth: " + hand.enemyobject.Enemyhealth);

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

        if (CurrentState == ObjectState.Rejecting && enemy.CurrentState == ObjectState.Flying)
        {
            var star = ObjectManager.CreateManagedObject<StarBloom>("StarBloom");
            star.SetStarColor(StarColor.Blue);

            MiniCore.Get<CharacterController>().Score += 5;
            star.transform.position = col.contacts[0].point;
            var boom = ObjectManager.CreateManagedObject<Boom3DText>("Boom3DText");
            boom.transform.position = col.contacts[0].point;
            boom.textMesh.text = "连续反弹";

            enemy.CurrentState = ObjectState.Rejecting;
            enemy.CachedRigidbody.velocity = (enemy.transform.position - transform.position).normalized * Random.Range(2, 3);
        }
      

    }
}
