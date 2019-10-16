using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManagerTest : MonoBehaviour
{

    public int i = 0;
    // Start is called before the first frame update
    void Start()
    {

    }
    void CreateTimer()
    {
        ObjectManager.CreateManagedObject("TestObj");
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {

    }
}
