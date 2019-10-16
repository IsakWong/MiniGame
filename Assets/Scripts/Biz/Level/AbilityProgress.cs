using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;


public class AbilityProgress : MonoBehaviour
{
    public GameObject StarPrefab;

    public Material unlight;
    public Material highlight;

    public int TargetHighlight = 0;


    public int CurrentHighlight = 0;
   

    public GameObject[] Stars;

    private void Awake()
    {
        Stars = new GameObject[12];
        for(int i = 0; i < 12;++i)
        {
            var starObj = GameObject.Instantiate(StarPrefab);
            Stars[i] = starObj;
            var pos = starObj.transform.position;
            pos.x = Mathf.Cos(i * 30 * Mathf.Deg2Rad) * 2f;
            pos.y = Mathf.Sin(i * 30 * Mathf.Deg2Rad) * 2f;
            starObj.transform.parent = transform;
            starObj.transform.position = pos;
            Stars[CurrentHighlight].GetComponent<ParticleSystem>().enableEmission = false;
        }
        InvokeRepeating("Highlight", 1, 0.3f);
        this.AddListener<float, float>(GlobalGameMessage.OnAbilityProgressChange, delegate (float old, float value)
          {
              float s = value / 100;
              TargetHighlight =(int)( s * (float)12);

          });
    }
    public void Highlight()
    {
        if(CurrentHighlight > TargetHighlight)
        {
            CurrentHighlight--;
            Stars[CurrentHighlight].GetComponent<SpriteRenderer>().material = unlight;
            Stars[CurrentHighlight].GetComponent<SpriteRenderer>().color = new Color(0.3f, 0.3f, 0.3f);
            Stars[CurrentHighlight].GetComponent<ParticleSystem>().enableEmission = false;
        }
        else
        {
            if(CurrentHighlight < TargetHighlight)
            {
                Stars[CurrentHighlight].GetComponent<SpriteRenderer>().material = highlight;
                Stars[CurrentHighlight].GetComponent<SpriteRenderer>().color = new Color(1,1, 1);
                Stars[CurrentHighlight].GetComponent<ParticleSystem>().enableEmission = true;
                CurrentHighlight++;
            }
        }
    }
}
