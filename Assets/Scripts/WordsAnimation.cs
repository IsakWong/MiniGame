using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WordsAnimation : MonoBehaviour
{
    public Image[] words;
    public int current = 0;
    public Sequence seq;
    // Start is called before the first frame update
    void Awake()
    {
        seq = DOTween.Sequence();
        seq.AppendCallback(delegate()
        {
            words[current].DOFade(1, 0.8f);
        });
        seq.AppendInterval(2.1f);
        seq.AppendCallback(delegate ()
        {
            words[current].DOFade(0, 0.5f); current++;
        });
        seq.SetLoops(words.Length);
    }

   
}
