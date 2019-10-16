using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WheelRotate : MonoBehaviour
{
    public Transform WheelSmall;
    public Transform WheelBig;
    public int i = 1;
    // Start is called before the first frame update
    void Start()
    {
        //WheelSmall.DORotate(new Vector3(0, 0, 360), 3.0f).SetLoops(-1, LoopType.Incremental);
        //WheelBig.DORotate(new Vector3(0, 0, 360), 6.0f).SetLoops(-1, LoopType.Incremental);
        StartCoroutine(RotIt());
    }

    IEnumerator RotIt()
    {
        while (true)
        {
            i++;
            WheelSmall.localRotation = Quaternion.Euler(0, 0, i % 360);
            WheelBig.localRotation = Quaternion.Euler(0, 0, -i % 360);
            yield return new WaitForSeconds(0.016f);
        }
        
    }
}
