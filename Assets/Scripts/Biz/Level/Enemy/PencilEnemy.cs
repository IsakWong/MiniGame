using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mini.Core;
using UnityEngine;

public class PencilEnemy : EnemyObject
{
    // Start is called before the first frame update

    private Tweener tweener = null;
    private float timer = 0.0f;

    public enum PencilState
    {
        Flying,
        Rotating
    }

    public override void OnPause()
    {
        base.OnPause();
        if (tweener != null && tweener.IsActive())
        {
            tweener.timeScale = 0;
        }

    }

    public override void OnRestore()
    {
        base.OnRestore();
        if (tweener != null && tweener.IsActive())
        {
            tweener.timeScale = 1f;
        }
    }

    private PencilState state = PencilState.Flying;
    public override void Init()
    {
        base.Init();

        CurrentState = ObjectState.Flying;
        gameObject.layer = LayerMask.NameToLayer("PencilFlying");
        timer = 0.0f;
        var pos = transform.position - MiniCore.Get<GameController>().CurrentWorld.Main.transform.position;
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
                    gameObject.layer = LayerMask.NameToLayer("Pencil");

                    MiniCore.Get<CharacterController>().Score += 5;
                    this.Get<CharacterController>().AbilityProgress += 10;

                    OnRejectWithHandEffect(hand, col);


                    Vector3 pos = Vector3.zero;

                    float px = transform.position.x;
                    float py = transform.position.y;
                    float abs = System.Math.Abs(py);

                    if (px > 0 && py > 0)
                    {
                        pos.x = -1.5f + -px / 2f;
                        pos.y = 2;
                    }

                    if (px < 0 && py > 0)
                    {
                        pos.x = 1.5f + -px / 2f;
                        pos.y = 2;
                    }

                    if (px < 0 && py < 0)
                    {
                        pos.x = 1.5f + -px / 2f;
                        pos.y = -2;
                    }

                    if (px > 0 && py < 0)
                    {
                        pos.x = -1.5f + -px / 2f;
                        pos.y = -2;
                    }

                    tweener = transform.DOMove(pos, 1.0f);
                    CachedRigidbody.velocity = Vector2.zero;
                    CachedRigidbody.angularVelocity = 360f;

                    if (hand.typechange)
                    {

                        CurrentState = ObjectState.Rejecting;
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

        if (enemy.GetType() == typeof(ChalkEnemy) || enemy.GetType() == typeof(PaperBallEnemy) || enemy.GetType() == typeof(DirtyWordsEnemy))
        {
            enemy.ObsorbedByMagicField(false);
        }

    }

    public void StopRotate()
    {
        if (tweener != null)
        {
            tweener.Kill(false);
            tweener = null;
            CurrentState = ObjectState.Flying;
            timer = 0;
        }
    }
    

    protected new void OnRecycle()
    {
        base.OnRecycle();
        if (tweener != null)
        {
            tweener.Kill(false);
            tweener = null;
        }
    }

    public void FixedUpdate()
    {
        base.FixedUpdate();

        if (MiniCore.Get<GameController>().IsPaused)
            return;

        if (CurrentState == ObjectState.Rejecting)
        {
            timer += Time.fixedDeltaTime;
            if (timer > 6.0f)
            {
                DestroyByMagicField();
            }
            else
            {

                CachedRigidbody.velocity = Vector2.zero;
                CachedRigidbody.angularVelocity = 360f;
            }
        }

    }

}
