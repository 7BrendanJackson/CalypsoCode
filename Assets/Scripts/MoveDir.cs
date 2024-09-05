using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Spins the wind turbine (Should implement this at a shader level if there is time)
public class MoveDir : MonoBehaviour
{
    [SerializeField] Vector3 dir;
    void Update()
    {
        transform.position += Time.deltaTime * dir;
    }
}
