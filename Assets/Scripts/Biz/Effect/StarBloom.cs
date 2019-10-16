using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum StarColor
{
    Blue,
    Yellow
}
public class StarBloom : ParticleObject
{
    public ParticleSystem Particle;
    public Color Blue;
    public Color Yellow;
    public void SetStarColor(StarColor color)
    {
        var main = Particle.main;
        switch (color)
        {
            case StarColor.Blue:
                main.startColor = new ParticleSystem.MinMaxGradient(Blue,Blue);
                break;
            case StarColor.Yellow:
                main.startColor = new ParticleSystem.MinMaxGradient(Yellow, Yellow);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(color), color, null);
        }
       

    }
}

