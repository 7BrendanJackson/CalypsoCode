using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

//The confirmation window for unlocking research
public class ResearchUnlockButton : MonoBehaviour
{

     public Button unlockButton;
    [SerializeField] TMP_Text nameResearch, descriptionText, popsNeededText, costText, buttonText;
    [SerializeField] Sprite lockedImage, unlockedImage, availableImage, displayImageSprite;
    [SerializeField] Image displayImage;
    [SerializeField] AudioSource uiAudioSource;
    [SerializeField] AudioClip successUnlockClip;

    //Receives info from the clicked node to display the details page
    public void PassInfo(ResearchNode node)
    {
        //Update the details window
        costText.text = node.cost.ToString();
        popsNeededText.text = node.popNeeded.ToString();
        nameResearch.text = node.researchDisplayName;
        descriptionText.text = node.researchDescription;

        if (displayImage != null) displayImage.sprite = displayImageSprite;

        //Sets the unlock button to the appropriate image
        if (node.CanBeUnlocked() && !ResearchManager.Instance.research[node.researchName]) //Can be unlocked
        {
            unlockButton.image.sprite = availableImage;
        }
        else if (ResearchManager.Instance.research[node.researchName]) //Has already been unlocked
        {
            unlockButton.image.sprite = unlockedImage;
        }
        else //Cannot unlock
        {
            unlockButton.image.sprite = lockedImage;
        }
    }

}
