using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Opens the specified URL in browser
//Used by events to open to educational resources
public class URL : MonoBehaviour
{
    [SerializeField] string url;

    [ContextMenu("Open URL")]
    public void OpenURL()
    {
        Application.OpenURL(url);
    }
}
