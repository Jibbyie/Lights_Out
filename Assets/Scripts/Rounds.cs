using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rounds : MonoBehaviour
{
    public LightSwitch lightSwitch;
    private float lightsTurnedOffCounter;

    void Start()
    {
        lightSwitch = FindObjectOfType<LightSwitch>();
        lightsTurnedOffCounter = lightSwitch.lightsTurnedOffCounter;
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
