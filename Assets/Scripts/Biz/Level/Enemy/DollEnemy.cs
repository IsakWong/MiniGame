using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class DollEnemy : EnemyObject
{
    //public int health = 5;
    public bool isfixed = false;

    public override void Init()
    {
        base.Init();
        transform.SetParent(null);
        CurrentState = ObjectState.Flying;
        Enemyhealth = 5;
        isfixed = false;

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
        StarBloom star = null;

        switch (CurrentState)
        {
            case ObjectState.Flying:

                if (hand.handType == HandType.RejectHand)
                {
                    CurrentState = ObjectState.Rejecting;
                    CaculateVelocity(hand);

                    if (hand.typechange)
                    {

                        hand.enemyobject.Enemyhealth = 5;
                        CurrentState = ObjectState.Obsorbed;
                    }
                    else
                    {

                        //Vector2 dir = transform.position - hand.transform.position;
                        //dir.Normalize();
                        //CachedRigidbody.velocity = dir * Random.Range(2f, 3f);
                        //CachedRigidbody.angularVelocity = Random.Range(90f, 360f);
                    }

                }
                if (hand.handType == HandType.AbsorbHand)
                {
                    
                    CachedRigidbody.velocity = Vector2.zero;

                    CachedRigidbody.angularVelocity = 0.0f;
                    CachedRigidbody.simulated = false;
                    gameObject.transform.SetParent(hand.GrabPoint.transform);
                    gameObject.transform.localPosition = Vector3.zero;

                    hand.enemyobject = this;
                    hand.handType = HandType.RejectHand;
                    hand.typechange = true;
                    isfixed = true;
                    ObjectManager.CreateManagedObject("SmokeBoom").transform.position = hand.GrabPoint.transform.position;
                    MainCharacter curCharacter = hand.MainCharacter;
                    ViewManager.GetView<LevelOverlayView>().ShowTip("用小熊抵挡飞行物，它会保护你。", 2.5f);
                    MiniCore.PlaySound("捡起小熊");
                    if (curCharacter.collideWithStar)
                    {
                        curCharacter.collideWithStar = false;
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
        Debug.Log("isfixed" + isfixed);
        if (CurrentState == ObjectState.Flying && isfixed)
        {

            Debug.Log("doll" + Enemyhealth);
            Enemyhealth--;

            if (Enemyhealth == 0)
            {
                CurrentState = ObjectState.Obsorbed;
            }
        }

    }
    protected new void OnRecycle()
    {
        base.OnRecycle();
        transform.SetParent(null);
    }
    private void FixedUpdate()
    {
        if (Enemyhealth <= 1)
        {
            this.GetComponent<SpriteRenderer>().color = new Color(1, 0.6f, 0.6f);
            if (Enemyhealth <= 0)
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
