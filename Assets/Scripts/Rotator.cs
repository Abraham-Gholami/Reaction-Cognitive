using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    float speed = 4;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate( Vector3.forward ,speed * Time.deltaTime, Space.Self);
    }
}
