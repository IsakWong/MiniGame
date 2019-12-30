using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mini.Core;
using UnityEngine;
using Random = UnityEngine.Random;


public enum EnemyType
{
    Bandage,
    Pencil,
    PaperBall,
    Book,
    Chalk,
    Candy,
    Doll,
    FunnyFaceBoss,
    DirtyWords,
    Star,
    HandEnemy,

    BlackCharacter,
    Unknown
}
public class EnemyObject : ManagedObject
{
    public EnemyType enemyType = EnemyType.Bandage;

    public Vector3 NormalScale = Vector3.zero;

    public int Enemyhealth = 0;

    public float MaxVelocitySize = 10;
    public float MaxAngularVelocity = 800;

    public static HashSet<EnemyObject> EnemyObjects = new HashSet<EnemyObject>();

    public enum ObjectState
    {
        Flying,
        Rejecting,
        Obsorbed,
        Destroyed
    }

    public Rigidbody2D CachedRigidbody;
    protected ObjectState _currentState = ObjectState.Flying;


    private Vector2 storedVelocity;
    private float storedAngleVelocity;

    public virtual void OnPause()
    {
        storedVelocity = CachedRigidbody.velocity;
        storedAngleVelocity = CachedRigidbody.angularVelocity;

        CachedRigidbody.velocity = CachedRigidbody.velocity * 0;
        CachedRigidbody.angularVelocity = CachedRigidbody.angularVelocity * 0;

    }

    public virtual void OnRestore()
    {
        CachedRigidbody.velocity = storedVelocity;
        CachedRigidbody.angularVelocity = storedAngleVelocity;
    }

    public ObjectState CurrentState
    {
        get { return _currentState; }
        set
        {
            switch (value)
            {
                case ObjectState.Flying:
                    break;
                case ObjectState.Rejecting:
                    break;
                case ObjectState.Obsorbed:
                    if (CachedRigidbody != null)
                    {
                        CachedRigidbody.velocity = Vector2.zero;
                        CachedRigidbody.angularVelocity = 0;
                    }
                    transform.DOScale(0, 0.2f).OnComplete(delegate () { Recycle(); });
                    break;
                case ObjectState.Destroyed:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
            _currentState = value;
        }
    }

    protected float flyVelocity = 2.0f;

    protected void CaculateVelocity(Hand hand)
    {
        Vector2 dir = transform.position - hand.transform.position;
        dir.Normalize();
        CachedRigidbody.velocity = dir * Random.Range(2f, 3f) * (MiniCore.Get<GameController>().CurrentWorld.Main.Speed * 2 + 1f) * MiniCore.TimeScale;
        CachedRigidbody.angularVelocity = Random.Range(90f, 360f);
    }

    // Start is called before the first frame update

    #region Biz相关的回调重载
    public virtual void Init()
    {
        transform.localScale = NormalScale;
    }
    public virtual void OnCollideWithBody(MainCharacter character, Collision2D col)
    {

    }
    public virtual void OnCollideWithHand(Hand hand, Collider2D col)
    {

    }
    public virtual void OnCollideWithEnemy(EnemyObject enemy, Collision2D col)
    {

    }
    public virtual void ExitCollideWithHand(Hand hand, Collision2D col)
    {
    }

    public virtual void ExitCollideWithEnemy(EnemyObject enemy, Collision2D col)
    {

    }
    #endregion
    void OnTriggerEnter2D(Collider2D col)
    {
        //Debug.Log(gameObject.ToString() + " Trigger with " + col.gameObject.ToString());
        if (col.gameObject.GetComponent<DestroyEnemy>() != null)
        {
            EnemyObject enemy = this; Debug.Log("碰到星星");
            if (enemy.GetType() == typeof(BookEnemy)
                || enemy.GetType() == typeof(PaperBallEnemy)
                || enemy.GetType() == typeof(PencilEnemy)
                || enemy.GetType() == typeof(ChalkEnemy))
            {
                ViewManager.GetView<LevelOverlayView>().ShowTip("魔法消除障碍物！", 2.5f);
                ObsorbedByMagicField(false);
                MiniCore.Get<CharacterController>().Score += 5;
            }
        }

    }
    void OnCollisionEnter2D(Collision2D col)
    {
        //Debug.Log(gameObject.ToString() + " Collide with " + col.gameObject.ToString());
        EnemyObject enemy = col.gameObject.GetComponent<EnemyObject>();
        if (enemy != null)
        {
            col.gameObject.GetComponent<EnemyObject>().OnCollideWithEnemy(this, col);
        }
    }
    void OnCollisionExit2D(Collision2D col)
    {
        //Debug.Log(gameObject.ToString() + " Collide Exit with " + col.gameObject.ToString());
        EnemyObject enemy = col.gameObject.GetComponent<EnemyObject>();
        if (enemy != null)
        {
            col.gameObject.GetComponent<EnemyObject>().ExitCollideWithEnemy(this, col);
        }
    }

    protected virtual void OnRejectWithHandEffect(Hand hand, Collider2D col)
    {
        this.Get<CharacterController>().Combo++;
        StarBloom star = ObjectManager.CreateManagedObject<StarBloom>("StarBloom");
        star.SetStarColor(StarColor.Blue);
        star.transform.position = transform.position;
        MiniCore.PlaySound("ding3");
        Camera.main.GetComponent<Shake>().shakeAmount = Random.Range(0.06f, 0.12f);
        Camera.main.GetComponent<Shake>().shakeDuration = Random.Range(0.12f, 0.17f);


    }

    public void ObsorbedByMagicField(bool shake = true)
    {
        if (CurrentState != ObjectState.Obsorbed)
        {

            var star = ObjectManager.CreateManagedObject("SmokeBoom");
            var boom = ObjectManager.CreateManagedObject<Boom3DText>("Boom3DText");
            boom.textMesh.text = "消失!";
            boom.transform.position = transform.position;
            star.transform.position = transform.position;
            CurrentState = ObjectState.Obsorbed;
            if (shake)
            {
                Camera.main.GetComponent<Shake>().shakeDuration = Random.Range(0.1f, 0.2f);
                Camera.main.GetComponent<Shake>().shakeAmount = Random.Range(0.1f, 0.15f);
            }
        }
    }

    public void DestroyByMagicField()
    {
        ObsorbedByMagicField(true);
    }

    #region Behaviour 重载函数
    protected void Awake()
    {
        base.Awake();
        NormalScale = transform.localScale;
        CachedRigidbody = GetComponent<Rigidbody2D>();
    }
    protected void OnEnable()
    {
        EnemyObjects.Add(this);
    }

    protected void OnDisable()
    {
        EnemyObjects.Remove(this);
        if (CachedRigidbody != null)
        {
            CachedRigidbody.velocity = Vector2.zero;
            CachedRigidbody.angularVelocity = 0;

            CachedRigidbody.simulated = true;

        }

    }
    protected override string GetCategory() 
    {
        return "[World.Enemy]";
    }
    #endregion

}
