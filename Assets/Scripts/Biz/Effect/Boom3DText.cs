using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;



public class Boom3DText : EffectObject
{
    public string SortingLayerName = "Default";
    public TextMesh textMesh;
    public int SortingOrder = 0;

    Sequence seq;

    protected  void Awake()
    {
        base.Awake();
        if (seq == null)
            seq = DOTween.Sequence().SetAutoKill(false);

        var meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.sortingLayerName = SortingLayerName;
        meshRenderer.sortingOrder = SortingOrder;

        seq.Append(transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutExpo));
        seq.AppendInterval(0.1f);
        seq.Append(transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack));
    }
    protected void OnDisable()
    {
    }

    protected void OnEnable()
    {
        transform.localScale = Vector3.zero;
        seq.Restart();
    }

}
