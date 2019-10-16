using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StartArrowAnimation : MonoBehaviour
{
    public Image[] Arrows = new Image[6];
    public int ptr = 0;
    public Sequence seq;
    // Start is called before the first frame update
    void OnEnable()
    {
        Color color = Arrows[0].color;
        color.a = 0;
        int i = 0;
        while (i < 6)
        {
            Arrows[i].color = color;
            i++;
        }
        StartCoroutine(LightUp(Arrows[0]));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator LightUp(Image arrow)
    {
        ptr++;
        float a = 0;
        while (a < 1)
        {
            a += 0.1f;
            Color color = arrow.color;
            color.a = a;
            arrow.color = color;
            yield return new WaitForSeconds(0.016f);
        }
        StartCoroutine(LightUp(Arrows[ptr % 6]));
        while (a > 0)
        {
            a -= 0.05f;
            Color color = arrow.color;
            color.a = a;
            arrow.color = color;
            yield return new WaitForSeconds(0.016f);
        }

    }

}
