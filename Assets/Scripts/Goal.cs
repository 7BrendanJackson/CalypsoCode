using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//Final goal to complete the game
//Used in displayed checklist of goals to eliminate climate change once discovered.
public class Goal : MonoBehaviour
{
    [SerializeField] TMP_Text 
        noPollution, 
        pops, 
        noCoal;

    [SerializeField] GameObject finale;
    GameManager gm;

    void Start()
    {
        gm = GameManager.Instance;
    }

    //Called at the start of every new turn
    public void CheckGoal()
    {
        if (gm.climateLevel == 0)
        {
            noPollution.color = Color.green;
            noPollution.fontStyle = FontStyles.Strikethrough;
        }
        else
        {
            noPollution.color = Color.white;
            noPollution.fontStyle = FontStyles.Normal;
        }

        if (gm.totalPopulation >= 100)
        {
            pops.color = Color.green;
            pops.fontStyle = FontStyles.Strikethrough;
        }
        else
        {
            pops.color = Color.white;
            pops.fontStyle = FontStyles.Normal;
        }

        if (GameObject.FindGameObjectsWithTag("Coal").Length == 0)
        {
            noCoal.color = Color.green;
            noCoal.fontStyle = FontStyles.Strikethrough;
        }
        else
        {
            noCoal.color = Color.white;
            noCoal.fontStyle = FontStyles.Normal;
        }
        if (noPollution.color == Color.green && pops.color == Color.green && noCoal.color == Color.green)
        {
            //Game currently goes to main menu on completion
            finale.SetActive(true);
        }
    }
}
