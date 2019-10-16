using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BlurPost : MonoBehaviour
{
    [SerializeField]
    public Material postprocessMaterial;

    public Material material = null;
    public bool NeedBlur = false;

    private Tweener tweener;
    public void BeginBlur()
    {
        if(material == null)
            material = new Material(postprocessMaterial);
        NeedBlur = true;
        material.SetFloat("_BlurSize", 0);
        tweener = material.DOFloat(0.02f, "_BlurSize", 0.3f);
    }
    public void EndBlur()
    {
        material.DOFloat(0.0f, "_BlurSize", 0.3f).OnComplete(delegate()
        {
            NeedBlur = false;
        });
    }
    // Start is called before the first frame update
 
}
