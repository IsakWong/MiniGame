using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WheelWidget : MonoBehaviour
{
    private Vector3 delta;
    private Vector3 first;

    private Vector3 old;

    public RectTransform Wheel;
    public RectTransform Start;

    // Start is called before the first frame update
    void Awake()
    {

        startTweener = Start.DOAnchorPos3DX(0, 0.1f).SetAutoKill(false).SetEase(Ease.InOutCubic);
        wheelTweener = Wheel.DORotate(new Vector3(0, 0, 0), 0.1f, RotateMode.Fast).SetEase(Ease.InOutCubic);
    }


    public Action OnTrigger;
    private Tweener wheelTweener;
    private bool pressed = false;
    private Tweener startTweener;
    // Update is called once per frame
    private void Update()
    {

        delta = Vector3.zero;

        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.OSXEditor:

                if (Input.GetMouseButtonDown(0))
                {
                    old = Input.mousePosition;
                    pressed = true;
                    first = Input.mousePosition;
                }
                if (pressed == false)
                {
                    break;
                }
                if (Input.GetMouseButtonUp(0))
                    {

                        if ((Input.mousePosition - first).x > 200)
                        {
                            if (!startTweener.IsPlaying())
                            {
                                if (!wheelTweener.IsPlaying())
                                {
                                    wheelTweener = Wheel.DORotate(new Vector3(0, 0, -360), 0.5f, RotateMode.WorldAxisAdd);
                                    startTweener = Start.DOAnchorPosX(1100, 0.5f);

                                    if (OnTrigger != null)
                                        OnTrigger.Invoke();
                                }

                            }

                        }
                        else
                        {
                            if (!startTweener.IsPlaying())
                            {
                                if (!wheelTweener.IsPlaying())
                                {
                                    wheelTweener = Wheel.DORotate(new Vector3(0, 0, 0), 0.5f);
                                    startTweener = Start.DOAnchorPosX(0, 0.5f);
                                }

                            }
                        }

                        break;


                    }
                    if (Input.GetMouseButton(0))
                    {
                        if (!startTweener.IsPlaying())
                        {
                            if (!wheelTweener.IsPlaying())
                            {
                                delta = Input.mousePosition - old;
                                old = Input.mousePosition;
                                var newPos = Start.anchoredPosition;
                                newPos.x += delta.x;
                                Start.anchoredPosition = newPos;
                                Wheel.transform.Rotate(0, 0, -delta.x * 0.2f);
                            }

                        }
                    }


                break;
            case RuntimePlatform.IPhonePlayer:
            case RuntimePlatform.Android:
                if (Input.touches.Length > 0)
                {
                    if (Input.touches[0].phase == TouchPhase.Began)
                    {
                        pressed = true;
                        old = Input.mousePosition;
                        first = Input.mousePosition;
                    }

                    if (pressed == false)
                        break;

                    if (Input.touches[0].phase == TouchPhase.Ended)
                    {
                        pressed = false;
                        if ((Input.mousePosition - first).x > 100)
                        {
                            if (!startTweener.IsPlaying())
                            {
                                if (!wheelTweener.IsPlaying())
                                {
                                    wheelTweener = Wheel.DORotate(new Vector3(0, 0, -360), 0.5f, RotateMode.WorldAxisAdd);
                                    startTweener = Start.DOAnchorPosX(1100, 0.5f);
                                    if (OnTrigger != null)
                                        OnTrigger.Invoke();
                                }

                            }

                        }
                        else
                        {
                            if (!startTweener.IsPlaying())
                            {
                                if (!wheelTweener.IsPlaying())
                                {
                                    wheelTweener = Wheel.DORotate(new Vector3(0, 0, 0), 0.5f);
                                    startTweener = Start.DOAnchorPosX(0, 0.5f);
                                }

                            }
                        }
                    }
                    if (Input.touches[0].phase == TouchPhase.Moved)
                    {
                        if (!startTweener.IsPlaying())
                        {
                            if (!wheelTweener.IsPlaying())
                            {
                                delta = Input.mousePosition - old;
                                old = Input.mousePosition;
                                var newPos = Start.anchoredPosition;
                                newPos.x += delta.x;
                                Start.anchoredPosition = newPos;
                                Wheel.transform.Rotate(0, 0, -delta.x * 0.2f);
                            }

                        }
                    }
                }
                break;
        }
    }
}
