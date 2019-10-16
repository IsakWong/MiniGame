using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarController : MonoBehaviour
{

    public MainCharacter main;
    public ControllerButtonCtrl controlButton;
    public float rolltime=16;
    public float totaltime=16;
    private float rate;
    public float movelength = 800;
    public GearCtrl GearCtrl;
   
    private void Awake()
    {
        transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform);
        transform.localPosition = new Vector3(0, 0, 0);
        GearCtrl = GameObject.FindGameObjectWithTag("GearCtrl").GetComponent<GearCtrl>();
        main = GameObject.FindGameObjectWithTag("Player").GetComponent<MainCharacter>();
    }
    // Start is called before the first frame update
    void Start()
    {
        rate = movelength / totaltime;
    }

    // Update is called once per frame
    void Update()
    {
        if (controlButton.Enabled)
        {

        }
        else
        {
            controlButton.transform.localPosition = new Vector3(rolltime * rate - movelength/2, -650, 0);
            
        }
        main.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -0.6f*controlButton.transform.localPosition.x));
        if (GearCtrl != null)
        {
            if (controlButton.transform.localPosition.x < 0)
            {
                GearCtrl.rot = (float)(System.Math.Acos(-controlButton.transform.localPosition.x/400)/System.Math.PI*180);
            }
            else
            {
                GearCtrl.rot = (float)(System.Math.Acos(-controlButton.transform.localPosition.x / 400) / System.Math.PI * 180);
            }
        }
        
    }
    private void FixedUpdate()
    {
        if (controlButton.Enabled)
        {
        }
        else
        {
            if (rolltime > 0)
            {
                rolltime -= Time.fixedDeltaTime;
            }
        }

    }
    public void SetRolltime(float x)
    {
        rolltime = (x + movelength/2) / rate;
    }

}
