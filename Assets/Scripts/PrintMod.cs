using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//Prints out the current modifiers about the population
//Pretty sure this isn't used for anything but the console gets flooded with warnings if removed
public class PrintMod : MonoBehaviour
{

    [SerializeField] TMP_Text text;


    private void OnEnable()
    {
        text.text = "";
        for (int i = 0; i < GameManager.Instance.happinessModifiers.Count; i++)
        {
            text.text += $"{GameManager.Instance.happinessModifiers[i].name} {GameManager.Instance.happinessModifiers[i].num} for {GameManager.Instance.happinessModifiers[i].time} Months";
        }
    }
}
