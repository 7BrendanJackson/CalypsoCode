using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Some useful commands not included in default Unity
//Need to port other commands in and change game menu to use these if time
public static class ExtensionMethods
{
    //Disbale the specified canvas overlay (Use for build menu, popups etc.)
    public static void DisableGroup(this CanvasGroup cg)
    {
        cg.alpha = 0;
        cg.blocksRaycasts = false;
        cg.interactable = false;
    }

    //Enable the specified canvas overlay
    public static void EnableGroup(this CanvasGroup cg)
    {
        cg.alpha = 1;
        cg.blocksRaycasts = true;
        cg.interactable = true;
    }

    //Could move vector3 inverse lerp here
}
