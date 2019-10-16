using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }
    void OnTriggerEnter2D(Collider2D col)
    {
        EnemyObject obj = col.gameObject.GetComponent<EnemyObject>();
        if (obj == null)
            return;
        switch (obj.enemyType)
        {
            case EnemyType.Bandage:
            case EnemyType.Pencil:
            case EnemyType.PaperBall:
            case EnemyType.Book:
            case EnemyType.Chalk:
            case EnemyType.HandEnemy:
            case EnemyType.DirtyWords:
                obj.CachedRigidbody.velocity = obj.CachedRigidbody.velocity * 0.1f;
                obj.CachedRigidbody.angularVelocity = obj.CachedRigidbody.angularVelocity * 0.1f;
                obj.Invoke("DestroyByMagicField", 0.5f);
                break;
            case EnemyType.Star:
            case EnemyType.BlackCharacter:
                break;
            case EnemyType.FunnyFaceBoss:
                break;
            case EnemyType.Unknown:
                break;
        }

    }
}
