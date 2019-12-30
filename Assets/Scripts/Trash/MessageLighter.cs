using System.Collections;
using System.Collections.Generic;
using Mini.Core;
using UnityEngine;

public class MessageLighter : MonoBehaviour
{
    public WorldLevel3 WorldLevel3;
    public GameObject Bottom;
    public SpriteRenderer wheel1;
    public SpriteRenderer wheel2;
    float i = 0;
    public static bool GO = false;
    public static bool showed = false;
    // Start is called before the first frame update

    private void OnEnable()
    {
        StartCoroutine(IncreaseColor());
        GO = false;
        showed = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (GO&&(!showed))
        {
            showed = true;
            StartCoroutine(CallButtom());
            StartCoroutine(AutoWin());
        }
    }
    IEnumerator IncreaseColor()
    {
        Color color = this.GetComponent<SpriteRenderer>().color;
        while (i<0.5)
        {
            i += 0.032f;
            color.a = i;
            this.GetComponent<SpriteRenderer>().color = color;
            yield return new WaitForSeconds(0.016f);
        }
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(DecreaseColor());
    }
    IEnumerator DecreaseColor()
    {
        Color color = this.GetComponent<SpriteRenderer>().color;
        while (i > 0)
        {
            i -= 0.032f;
            color.a = i;
            this.GetComponent<SpriteRenderer>().color = color;
            yield return new WaitForSeconds(0.016f);
        }

        StartCoroutine(IncreaseColor());
    }

    IEnumerator CallButtom()
    {

        //召唤底座
        Bottom.SetActive(true);
        Color color = Color.white;
        color.a = 0;
        Bottom.GetComponent<SpriteRenderer>().color = color;
        wheel1.color = color;
        wheel2.color = color;
        //圆盘由小变大动画
        float s = 0;
        while (s < 0.328f)
        {
            s += 0.008f;
            Bottom.transform.localScale = new Vector3(s, s, 1);
            yield return new WaitForSeconds(0.032f);
        }
        //底座与齿轮出现
        float a = 0;
        while (a < 1f)
        {
            a += 0.016f;
            color.a = a;
            Bottom.GetComponent<SpriteRenderer>().color = color;
            wheel1.color = color;
            wheel2.color = color;
            yield return new WaitForSeconds(0.016f);
        }
    }
    IEnumerator AutoWin()
    {
        yield return new WaitForSeconds(3f);
        MiniCore.Get<GameController>().Win();
        gameObject.SetActive(false);
    }
}
