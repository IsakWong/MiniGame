using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class StartSceneAnimation : MonoBehaviour
{
    public RawImage BGlight;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(IncreaseColor(this.GetComponent<RawImage>()));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator IncreaseColor(RawImage raw)
    {
        float i = 0;
        Color color = Color.white;
        while (i < 0.95)
        {
            i += Random.Range(0, 0.016f);
            color.a = i;
            raw.color = color;
            yield return new WaitForSeconds(0.032f);
        }
        StartCoroutine(DecreaseColor(raw));
    }
    IEnumerator DecreaseColor(RawImage raw)
    {
        float i = 1;
        Color color = Color.white;
        while (i > 0.032)
        {
            i -= Random.Range(0, 0.016f);
            color.a = i;
            raw.color = color;
            yield return new WaitForSeconds(0.032f);
        }

        StartCoroutine(IncreaseColor(raw));
    }

}
