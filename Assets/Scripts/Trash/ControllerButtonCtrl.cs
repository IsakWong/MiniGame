using System.Collections;
using System.Collections.Generic;
using System;
using Mini.Core;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControllerButtonCtrl : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler,
    IEndDragHandler
{
    public bool Enabled;
    public RectTransform canvas;          //得到canvas的ugui坐标
    private RectTransform imgRect;        //得到图片的ugui坐标
    Vector2 offset = new Vector3();    //用来得到鼠标和图片的差值
    BarController BarController;
    // Use this for initialization
    void Start()
    {
        canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>();
        imgRect = GetComponent<RectTransform>();
        BarController=this.transform.parent.GetComponent<BarController>();
        
    }

    //当鼠标按下时调用 接口对应  IPointerDownHandler
    public void OnPointerDown(PointerEventData eventData)
    {

        Vector2 mouseDown = eventData.position;    //记录鼠标按下时的屏幕坐标
        Vector2 mouseUguiPos = new Vector2();   //定义一个接收返回的ugui坐标
        //RectTransformUtility.ScreenPointToLocalPointInRectangle()：把屏幕坐标转化成ugui坐标
        //canvas：坐标要转换到哪一个物体上，这里img父类是Canvas，我们就用Canvas
        //eventData.enterEventCamera：这个事件是由哪个摄像机执行的
        //out mouseUguiPos：返回转换后的ugui坐标
        //isRect：方法返回一个bool值，判断鼠标按下的点是否在要转换的物体上
        bool isRect = RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, mouseDown, eventData.enterEventCamera, out mouseUguiPos);
        if (isRect)   
        {
            Enabled = true;
            StartCoroutine(CallGearSound());
            //计算图片中心和鼠标点的差值
            offset = imgRect.anchoredPosition - mouseUguiPos;
        }
    }

    //当鼠标拖动时调用   对应接口 IDragHandler
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 mouseDrag = eventData.position;   //当鼠标拖动时的屏幕坐标
        Vector2 uguiPos = new Vector2();   //用来接收转换后的拖动坐标
        //和上面类似
        bool isRect = RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, mouseDrag, eventData.enterEventCamera, out uguiPos);

        if (isRect)
        {
            //不改变Y轴
            Vector2 pos = new Vector2(offset.x + uguiPos.x, -600);
            //设置图片的ugui坐标与鼠标的ugui坐标保持不变
            //设置边界
            if(pos.x < -400)
            {
                pos.x = -400;
            }
            if (pos.x > 400)
            {
                pos.x = 400;
            }
            imgRect.anchoredPosition = pos;
            
        }
    }

    //当鼠标抬起时调用  对应接口  IPointerUpHandler
    public void OnPointerUp(PointerEventData eventData)
    {
        offset = Vector2.zero;
        Enabled = false;
        //发送坐标
        BarController.SetRolltime(this.transform.localPosition.x);
    }

    //当鼠标结束拖动时调用   对应接口  IEndDragHandler
    public void OnEndDrag(PointerEventData eventData)
    {
        offset = Vector2.zero;
       
    }
    IEnumerator CallGearSound()
    {
        MiniCore.MuteBgm();
        while (Enabled)
        {
            MiniCore.PlaySound("齿轮转动_短");
            yield return new WaitForSeconds(0.99f);
        }
        if (imgRect.anchoredPosition.x > -399)
        {
            MiniCore.ContinueBgm();
        }
    }
    void Update()
    {
        if (imgRect.anchoredPosition.x <= -399)
        {
            MiniCore.MuteBgm();
        }
    }
}
