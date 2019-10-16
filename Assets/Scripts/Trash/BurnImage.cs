using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BurnImage : MonoBehaviour
{
    public Material material;
    private Material localMaterial;
    // Start is called before the first frame update
    private void Awake()
    {
        localMaterial = new Material(material);
        localMaterial.SetFloat("_Progress", 1.0f);
        this.GetComponent<RawImage>().material = localMaterial;
    }

    public void Reset()
    {
        localMaterial.SetFloat("_Progress", 1.0f);
        this.GetComponent<RawImage>().material = localMaterial;
    }
    public void Fire()
    {
        localMaterial.DOFloat(0, "_Progress", 1.0f).OnComplete(delegate()
        {
            this.gameObject.SetActive(false);
        });
        
    }

}
