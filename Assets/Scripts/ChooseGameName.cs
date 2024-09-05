using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Allows the player to pick a name for the Island and Population
public class ChooseGameName : MonoBehaviour
{
    GameManager gm;
    [SerializeField] Image 
        islandInput, 
        popsInput;

    private void Start()
    {
        gm = GameManager.Instance;
    }

    public bool CheckNames()
    {
        RestoreTextColor();

       //print($"{gm.popsName} | {gm.islandName}");

        //Need to check for "" since clicking in and out of the text box makes it blank yet not technically null
        //Name isn't blank and not default
        if (gm.islandName == null || gm.islandName == "Calypso!" || gm.islandName == "")
        {
            islandInput.color = Color.red;
            return false;
        }
        if (gm.popsName == null || gm.popsName == "Quibbles!" || gm.popsName == "")
        {
            popsInput.color = Color.red;
            return false;
        }

        return true;

    }

    //Make sure the input color is restored on success
    public void RestoreTextColor()
    {
        popsInput.color = Color.white;
        islandInput.color = Color.white;
    }
}
