using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

using Random = UnityEngine.Random;


public class HeartEnemy : EnemyObject
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
        //不会跟身体碰撞

    }

    public override void OnCollideWithHand(Hand hand, Collider2D col)
    {
        //不会跟手碰撞
    }

    public override void OnCollideWithEnemy(EnemyObject enemy, Collision2D col)
    {
        base.OnCollideWithEnemy(enemy, col);

        if (enemy.GetType() == typeof(ChalkEnemy) || enemy.GetType() == typeof(PencilEnemy)
        || enemy.GetType() == typeof(BookEnemy) || enemy.GetType() == typeof(PaperBallEnemy))
        {
            CurrentState = ObjectState.Obsorbed;
            enemy.CurrentState = ObjectState.Obsorbed;
        }
    }
}
