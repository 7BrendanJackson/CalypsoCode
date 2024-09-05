using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Baked into the debug menu so can't be deleted
public class ToggleClick : MonoBehaviour
{
    public void Toggle(GameObject target)
    {
        target.SetActive(!target.activeSelf);
    }
}
