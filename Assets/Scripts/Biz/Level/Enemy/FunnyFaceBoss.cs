using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mini.Core;
using UnityEngine;
using Random = UnityEngine.Random;

public class FunnyFaceBoss : EnemyObject
{
    private SpriteRenderer spriteRenderer;
    private Tweener hitTweener;

    public GameObject GeneratePoint;


    private Sequence bossMoveSeq;
    private Sequence bossLoopMoveSeq;
    private Sequence generateSequence;


    public override void OnPause()
    {
        base.OnPause();
        if (bossMoveSeq != null)
            bossMoveSeq.timeScale = 0;
        if (bossLoopMoveSeq != null)
            bossLoopMoveSeq.timeScale = 0;
        if (generateSequence != null)
            generateSequence.timeScale = 0;
        if (hitTweener != null)
            hitTweener.timeScale = 0;
    }

    public override void OnRestore()
    {
        base.OnRestore();
        if (bossMoveSeq != null)
            bossMoveSeq.timeScale = 1f;
        if (bossLoopMoveSeq != null)
            bossLoopMoveSeq.timeScale = 1f;
        if (generateSequence != null)
            generateSequence.timeScale = 1f;
        if (hitTweener != null)
            hitTweener.timeScale = 1f;
    }
    public override void Init()
    {
        base.Init();

        transform.position = new Vector3(0, 8, 0);
        bossMoveSeq.Restart();
    }

    protected void OnDisable()
    {
        base.OnDisable();
        generateSequence.Pause();
        bossMoveSeq.Pause();
        bossLoopMoveSeq.Pause();
    }
    public void TakeHit()
    {
        hitTweener.Restart();
    }
    public void Stop()
    {
        bossMoveSeq.Pause();
        bossLoopMoveSeq.Pause();
        generateSequence.Pause();
    }

    private void GenerateDirtyWord()
    {
        var obj = MiniCore.Get<GameController>().CurrentWorld.CreateEnemyObjectWithoutInit<DirtyWordsEnemy>("DirtyWordsEnemy");
        obj.transform.position = GeneratePoint.transform.position;
        obj.CachedRigidbody.velocity = (Vector3.zero - obj.transform.position.normalized) * Random.Range(2f, 3f) * MiniCore.TimeScale;
        obj.Init();
    }
    protected void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();

        hitTweener = spriteRenderer.DOColor(Color.red, 0.1f).SetLoops(2, LoopType.Yoyo).SetAutoKill(false).Pause();

        bossLoopMoveSeq = DOTween.Sequence().Pause().SetAutoKill(false);
        
        //Boss生成物剧本
        generateSequence = DOTween.Sequence().Pause().SetAutoKill(false);
        generateSequence.AppendInterval(0.5f);
        generateSequence.AppendCallback(GenerateDirtyWord);
        generateSequence.AppendInterval(1f);
        generateSequence.AppendCallback(GenerateDirtyWord);
        generateSequence.AppendInterval(1f);
        generateSequence.AppendCallback(GenerateDirtyWord);
        generateSequence.AppendInterval(1f);
        generateSequence.AppendCallback(GenerateDirtyWord);
        generateSequence.AppendInterval(1f);
        generateSequence.AppendCallback(GenerateDirtyWord);
        generateSequence.AppendInterval(1f);
        generateSequence.AppendCallback(GenerateDirtyWord);
        generateSequence.AppendInterval(1f);
        generateSequence.AppendCallback(GenerateDirtyWord);
        generateSequence.AppendInterval(1f);
        generateSequence.AppendCallback(GenerateDirtyWord);
        generateSequence.AppendInterval(1f);
        generateSequence.AppendCallback(GenerateDirtyWord);
        generateSequence.AppendInterval(1f);
        generateSequence.SetLoops(-1);


        //Boss移动剧本

        bossLoopMoveSeq.AppendInterval(1f);
        bossLoopMoveSeq.Append(transform.DOMove(new Vector3(0, 5, 0), 1.0f));
        bossLoopMoveSeq.AppendInterval(1f);
        bossLoopMoveSeq.Append(transform.DOMove(new Vector3(3, 5, 0), 1.0f));
        bossLoopMoveSeq.AppendInterval(1.0f);
        bossLoopMoveSeq.Append(transform.DOMove(new Vector3(0, 5, 0), 1.0f));
        bossLoopMoveSeq.AppendInterval(1.0f);
        bossLoopMoveSeq.Append(transform.DOMove(new Vector3(-3, 5, 0), 1.0f));
        bossLoopMoveSeq.SetLoops(-1);


        bossMoveSeq = DOTween.Sequence().Pause().SetAutoKill(false);
        bossMoveSeq.AppendCallback(delegate ()
        {
            MiniCore.PlaySound("Boss出场嘲笑声");
        });
        bossMoveSeq.Append(transform.DOMove(new Vector3(0, 5f, 0), 1.5f));
        bossMoveSeq.AppendInterval(1f);
        bossMoveSeq.Append(transform.DOMove(new Vector3(-3, 5f, 0), 1.0f));
        bossMoveSeq.AppendCallback(delegate()
        {
            generateSequence.Restart();
            bossLoopMoveSeq.Restart();
        });


    }
    public override void OnCollideWithBody(MainCharacter character, Collision2D col)
    {
        base.OnCollideWithBody(character, col);

    }

    public override void OnCollideWithHand(Hand hand, Collider2D col)
    {
        base.OnCollideWithHand(hand, col);
    }

    public override void OnCollideWithEnemy(EnemyObject enemy, Collision2D col)
    {
        base.OnCollideWithEnemy(enemy, col);
        if (enemy.CurrentState == ObjectState.Rejecting)
        {
            TakeHit();
            var boom = ObjectManager.CreateManagedObject<Boom3DText>("Boom3DText");
            boom.transform.position = enemy.transform.position;
            boom.textMesh.text = "-5";
            MiniCore.PlaySound("Boss受击");
        }
    }
}
