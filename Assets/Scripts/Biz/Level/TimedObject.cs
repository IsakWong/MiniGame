using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedObject : ManagedObject
{
    public float LifeTime = 2;
    private float _passedLife = 0;

    // Update is called once per frame
    void FixedUpdate()
    {
        _passedLife += Time.fixedDeltaTime;
        if (_passedLife > LifeTime)
        {
            _passedLife = 0;
            Recycle();
        }
    }
}
