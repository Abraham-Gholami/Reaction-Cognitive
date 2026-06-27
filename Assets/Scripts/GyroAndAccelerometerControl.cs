using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GyroAndAccelerometerControl : MonoBehaviour
{
    private AccelerometerUtil accelerometerUtil;

    // Use this for initialization
    // Start is called before the first frame update
    void Start()
    {
        accelerometerUtil = new AccelerometerUtil();

        Input.gyro.enabled = true;
        ObjectMover(Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {
        if(useGyro)
        {
            ObjectMover(Input.gyro.userAcceleration);
        }
        else 
        {
            ObjectMover(accelerometerUtil.LowPassFiltered());

        }

        if(Application.platform == RuntimePlatform.Android)
        {
            CalculateVelocity();
            GetDistance();
            GetTraveledDistance();
        }

    }
    Vector3  velocity , initVelocity = Vector3.zero ,distance, startingPos = Vector3.zero  ;
    float time;
    void CalculateVelocity()
    {
        var temp = GetAcceleration();
        if(CheckVector(temp))
        {
            velocity = CalculateSpeed(temp,time);
            initVelocity = velocity;
            Debug.text =" Velocity is " + velocity;
            time += Time.deltaTime;



        }
        else 
        {
            velocity =  Vector3.zero;
            initVelocity = velocity;
            Debug.text =" Velocity is zero";
            time = 0;
        }
        
    }
    void GetDistance()
    {
        var velocityAbs = new Vector3 (Mathf.Abs(velocity.x),Mathf.Abs(velocity.y),Mathf.Abs(velocity.z)) ;
        distance +=  velocityAbs * time;
        velocityT.text = " Movement volume : " + distance / 2000;

        
    }
    bool CheckVector(Vector3 temp)
    {
        float x = temp.x;
        float y = temp.y;
        float z = temp.z;
        if(x >= 0.01f || y >= 0.01f || z >= 0.01f )
        {
            
            return true;
            
           
        }
        else if(x <= -0.01f || y <= -0.01f || z <= -0.01f )
        {
            return true;
        }
        else 
        {
            return false;
        }
    }
    Vector3 GetAcceleration()
    {
        Vector3 temp;
        if(useGyro)
        {
            temp = Input.gyro.userAcceleration;

        }else 
        {
            temp = accelerometerUtil.LowPassFiltered();

        }
        accelerationT.text =  " Gyroscope input : " + Input.gyro.userAcceleration;
        Acc2.text ="AcceleroMeter input : " + accelerometerUtil.LowPassFiltered();
        return temp;
    }
    float dist;
    void GetTraveledDistance()
    {
        dist = Vector3.Distance(distance,startingPos);
        dist = Mathf.Round(dist) / 2000;
        traveledDistanceT.text = "Covered Distance : " + dist.ToString("f1") + " M ";

    }
    public Text traveledDistanceT,velocityT,accelerationT,Acc2,Debug,currentInputT;
    void ObjectMover(Vector3 temp)
    {
        temp.z = 0;
        Object.transform.position += (temp);
    }
    public void UseGyro(bool value)
    {
        useGyro = value;
        Object.transform.position = Vector3.zero;
        distance = Vector3.zero;

    }
    Vector3 CalculateSpeed(Vector3 accel,float timeV)
    {
        Vector3 initSpeed = initVelocity;
        float x  = initSpeed.x + accel.x + timeV * Time.deltaTime;
        float y  = initSpeed.y + accel.y + timeV * Time.deltaTime;
        float z  = initSpeed.z + accel.z + timeV * Time.deltaTime;
        Vector3 finalSpeed = new Vector3(x,y,z);
        time = 0;
        return finalSpeed;



    }
    public GameObject Object;
    bool useGyro;
}
