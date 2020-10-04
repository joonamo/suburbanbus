using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusPlayerControl : MonoBehaviour
{
    public Bus targetBus;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        targetBus.accelerator = Input.GetAxis("Accelerate");
        targetBus.steering = Input.GetAxis("Steering");
    }
}
