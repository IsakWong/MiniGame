using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class PaperBallEnemy : EnemyObject
{
    private Tweener tweener = null;
    public override void Init()
    {
        base.Init();
        CurrentState = ObjectState.Flying;
        collideCount = 0;

    }

    public override void OnPause()
    {
        base.OnPause();
        if (tweener != null && tweener.IsActive())
            tweener.timeScale = 0;
    }

    public override void OnRestore()
    {
        base.OnRestore();
        if(tweener != null && tweener.IsActive())
            tweener.timeScale = 1.0f;
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

                if (collideCount >= 2)
                {
                    if (CurrentState == ObjectState.Flying)
                    {
                        CurrentState = ObjectState.Rejecting;
                        Vector3 dir = transform.position - hand.transform.position;
                        dir.z = 0;
                        dir.Normalize();
                        CachedRigidbody.angularVelocity = 100.0f;
                        CachedRigidbody.velocity = dir * Random.Range(2.0f, 4.0f);
                        MiniCore.Get<CharacterController>().Score += 5;
                        OnRejectWithHandEffect(hand, col);
                    }
                  

                }
                else
                {
                    if ( CurrentState == ObjectState.Flying)
                    {
                        if (hand.handType == HandType.RejectHand)
                        {
                            collideCount++;
                            OnRejectWithHandEffect(hand, col);
                            PlaySound();
                            Vector3 dir = transform.position - hand.transform.position;
                            dir.z = 0;
                            dir.Normalize();
                            CachedRigidbody.angularVelocity = 100.0f;
                            Vector3 pos = transform.position + dir * Random.Range(2.0f, 4.0f);
                            CurrentState = ObjectState.Rejecting;
                            tweener = transform.DOMove(pos, 1.0f).OnComplete(delegate ()
                            {
                                CurrentState = ObjectState.Flying;
                                CachedRigidbody.velocity = (Vector3.zero - transform.position).normalized * Random.Range(3f,5f);
                            });
                            tweener.timeScale = MiniCore.TimeScale;

                            if (hand.typechange)
                            {

                                collideCount++;
                                if (hand.enemyobject.Enemyhealth == 0)
                                {
                                    hand.handType = HandType.AbsorbHand;
                                    hand.typechange = false;
                                    hand.enemyobject.DestroyByMagicField();

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
        //GameObject star = null;

    }


    public int collideCount = 0;
    void PlaySound()
    {
        this.GetComponent<AudioSource>().Play();
    }
}
