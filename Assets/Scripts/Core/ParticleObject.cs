using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleObject : ManagedObject
{
    public void OnParticleSystemStopped()
    {
        Recycle();
    }
}
