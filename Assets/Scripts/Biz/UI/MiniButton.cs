using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MiniButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Tweener buttonTweener;
    void Awake()
    {
        buttonTweener = transform.DOScale(new Vector3(1.4f,1.4f,1.4f), 0.2f).Pause().SetAutoKill(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonTweener.PlayForward();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonTweener.PlayBackwards();
    }
}
