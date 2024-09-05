using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Quickly turns any text into a billboard without needing to modify the shader
public class Billboard : MonoBehaviour
{
    Transform _camera;
    void Start()
    {
        _camera = Camera.main.transform;
    }

    void LateUpdate()
    {
        transform.LookAt(_camera);
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
    }
}
