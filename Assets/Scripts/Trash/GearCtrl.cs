using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearCtrl : MonoBehaviour
{
    public GameObject Gears;
    public GameObject Handle;
    public float rot = 0;
    public WheelRotate WheelRotate;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (rot > 0.01f)
        {
            Gears.transform.localRotation = Quaternion.EulerAngles(0, rot / 180 * (float)System.Math.PI, 0);
            Handle.transform.localRotation = Quaternion.EulerAngles(0, (rot - 12) / 180 * (float)System.Math.PI, 0);
        }

        //Gears.transform.eulerAngles = new Vector3(0, rot, 0);
        //Handle.transform.eulerAngles = new Vector3(0, rot - 12, 0);

        if (WheelRotate != null) WheelRotate.i = (int)rot / 1;
    }
}
