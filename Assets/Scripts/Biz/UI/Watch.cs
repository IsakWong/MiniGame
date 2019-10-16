using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.UI;

public class Watch : MonoBehaviour
{

    public RawImage img1;
    public RawImage Min;
    public RawImage Hour;
    public Text Time;
    public int CurHour = 9;
    public int CurMinute = 0;

    public bool move = false;
    public Color color;

    private Sequence sequence;

    private GameController controller;
    // Start is called before the first frame update
    void Awake()
    {
        controller = this.Get<GameController>();

        Hour.transform.eulerAngles = new Vector3(0, 0, -30f * CurHour);
        Min.transform.eulerAngles = new Vector3(0, 0, 6 * CurMinute);
    }

    public void RotateHour()
    {
        controller = this.Get<GameController>();
        float delta = -10;
        if (controller.WorldIndex == 3 || controller.WorldIndex == 4)
        {
            delta = -30;
        }
        if (controller.WorldIndex == 1)
        {
            delta = -20;
        }
        MiniCore.PlaySound("齿轮");
        sequence = DOTween.Sequence();
        sequence.AppendInterval(0.6f);
        sequence.Append(Hour.transform.DORotate(new Vector3(0, 0, delta), 0.2f, RotateMode.LocalAxisAdd));
        sequence.AppendInterval(0.6f);
        sequence.Append(Hour.transform.DORotate(new Vector3(0, 0, delta), 0.2f, RotateMode.LocalAxisAdd));
        sequence.AppendInterval(0.6f);
        sequence.Append(Hour.transform.DORotate(new Vector3(0, 0, delta), 0.2f, RotateMode.LocalAxisAdd));
        sequence.AppendInterval(0.15f);
        sequence.AppendCallback(
            delegate()
            {
                if (controller.WorldIndex + 1 < controller.Hours.Length)
                {
                    Time.text = string.Format("{0} {1} : 0 0", (int)(controller.Hours[controller.WorldIndex + 1] / 10), controller.Hours[controller.WorldIndex + 1] % 10);
                }
                MiniCore.PlaySound("时钟");
            });
    }

    public void UpdateWatch()
    {
        Hour.transform.eulerAngles = new Vector3(0, 0, -30f * CurHour);
        Min.transform.eulerAngles = new Vector3(0, 0, 6 * CurMinute);
        Time.text = string.Format("{0} {1} : 0 0", (int)(CurHour / 10),CurHour%10);
    }

    public void FixedUpdate()
    {

        if (move)
        {
            if (controller.CurrentWorld != null)
            {
                float s = (controller.CurrentWorld.timeSinceLevelBegin / controller.CurrentWorld.WinTime);
                Min.transform.eulerAngles = new Vector3(0, 0, s * -360);
                if (controller.CurrentWorld.Hour > 12)
                {
                    Hour.transform.eulerAngles = new Vector3(0, 0,
                        (controller.CurrentWorld.Hour - 12) * (-30f) - s * 30f);
                }
                else
                {
                    Hour.transform.eulerAngles = new Vector3(0, 0,
                        controller.CurrentWorld.Hour * (-30f) + s * 30f);

                }

            }
        }
    }
    public void SetAlpha(float value)
    {
        color.a = value;
        if (img1 != null)
            img1.color = color;
        if (Min != null)
            Min.color = color;
        if (Hour != null)
            Hour.color = color;
        if (Time != null)
            Time.color = color;
    }
}
