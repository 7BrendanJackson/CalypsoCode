using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Used to spin wind turbine
//Should move to shader if there is time 
public class Rotator : MonoBehaviour
{
    [SerializeField] float speed;

    void Update()
    {
        transform.Rotate(Vector3.up, Time.deltaTime * 720, Space.Self);
    }
}
