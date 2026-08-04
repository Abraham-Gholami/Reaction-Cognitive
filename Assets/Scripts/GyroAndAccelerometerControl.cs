using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GyroAndAccelerometerControl : GenericSingleton<GyroAndAccelerometerControl>
{
    private AccelerometerUtil accelerometerUtil;
    void Start()
    {
        accelerometerUtil = new AccelerometerUtil();
        Input.gyro.enabled = true;
    }

    void Update()
    {
        if(Application.platform == RuntimePlatform.Android)
            GetAcceleration();
    }
    void GetAcceleration()
    {
        gyroT.text =  " Gyroscope input : " + Input.gyro.userAcceleration;
        accelerationT.text ="AcceleroMeter input : " + accelerometerUtil.LowPassFiltered();
    }
    public Text accelerationT,gyroT;
}
