using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public enum HandType
{
    RejectHand,
    AbsorbHand
}
public class Hand : MonoBehaviour
{
    public MainCharacter MainCharacter;
    public Light light;
    //250,234,100
    public HandType originHandType;
    public HandType handType;
    public EnemyObject enemyobject;
    public bool typechange = false;

    public GameObject GrabPoint;
    // Start is called before the first frame update
    void Start()
    {
        typechange = false;
    }

    public void ResetHand()
    {

        handType = originHandType;
        enemyobject = null;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //.Log(gameObject.ToString() + " Trigger with" + col.gameObject.ToString());
        EnemyObject obj = col.gameObject.GetComponent<EnemyObject>();
        if (obj != null)
        {
            obj.OnCollideWithHand(this, col);
        }
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log(gameObject.ToString() + " Collide with" + col.gameObject.ToString());

    }

    void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log(gameObject.ToString() + " Collide Exit with" + collision.gameObject.ToString());
        EnemyObject obj = collision.gameObject.GetComponent<EnemyObject>();
        if (obj != null)
        {

            obj.ExitCollideWithHand(this, collision);
        }
    }

}
