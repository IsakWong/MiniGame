using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EffectObject : ManagedObject
{
    [Header("计时器生命周期")]
    public float LifeTime = 2;

    [Header("粒子结束是否回收")]
    public bool RecycleOnParticleStopped = false;

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
    public void OnParticleSystemStopped()
    {
        if(RecycleOnParticleStopped)
            Recycle();
    }
    protected override string GetCategory()
    {
        return "[Effect]";
    }
}
