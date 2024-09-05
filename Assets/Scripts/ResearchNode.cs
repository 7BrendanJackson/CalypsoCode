using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Used on the research tree to display info about upgrades and processes the requirements before unlocking them
public class ResearchNode : MonoBehaviour
{
    Button button;
    [SerializeField] Sprite icon;
    Image popupIcon;
    public string researchName, researchDisplayName, researchDescription;
    public bool hidden;
    public int cost, popNeeded;
    public ResearchNode[] dependencies;
    CanvasGroup cg;
    [SerializeField] TMP_Text costText, popsNeededText;
    public ResearchUnlockButton unlockButton;

    [SerializeField] ColorBlock[] colors;//0=Unavailable | 1=Available | 2=Unlocked

    private void Awake()
    {
        //Initialize UI
        popupIcon = GameObject.FindGameObjectWithTag("PopupIcon").GetComponent<Image>();
        button = GetComponent<Button>();
        button.onClick.AddListener(OpenWindow);
        colors[1] = button.colors;
        costText.text = cost.ToString();
        popsNeededText.text = popNeeded.ToString();
        cg = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        //Attach to the research manager
        ResearchManager.Instance.updateNodes.AddListener(UpdateState);
        //Replace later with better connection
        unlockButton = GameObject.FindGameObjectWithTag("ResearchUnlockButton").GetComponent<ResearchUnlockButton>();

        UpdateState();
    }

    //Updates the current state of the node
    //Has it been unlocked for research?
     void UpdateState()
    {
        //Has climate change been discovered?
        if (hidden && !GameManager.Instance.discoveredClimateChange) //If not hide UI
        {
            cg.alpha = 0;
            cg.blocksRaycasts = false;
            cg.interactable = false;
            return;
        }
        else //If yes show UI
        {
            cg.alpha = 1;
            cg.blocksRaycasts = true;
            cg.interactable = true;
        }
        //Is already researched
        if (ResearchManager.Instance.research[researchName])
        {
            button.colors = colors[2];
            //button.interactable = true;
        }
        //Is Available to research
        else if (CanBeUnlocked())
        {
            button.colors = colors[1];
        }
        //Not Available to be researched (locked)
        else
        {
            //button.interactable = false;
            //Greyed out if dependency fails, red if succeeds
            button.colors = colors[DependencyUnlocked() ? 0 : 3];
        }

    }

    //Called when the research node is attempting to be unlocked
    public void Unlock()
    {
        if (!CanBeUnlocked())
        {
            //Add denied sound
            return;
        }

        //Deduct cost from stored credits
        GameManager.Instance.researchCredits -= cost;

        //Toggle to unlocked and activate
        ResearchManager.Instance.research[researchName] = true;
        ResearchManager.Instance.updateNodes.Invoke();

        //print($"{researchName}: Unlocked!");
        //Close current window
        CanvasGroup cg = unlockButton.transform.parent.gameObject.GetComponent<CanvasGroup>();
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        //Replace with better reference
        GameObject.FindGameObjectWithTag("ResearchUnlockPlayer").GetComponent<AudioSource>().Play();

        ResearchManager.Instance.InstallHouseUpgrades();
    }

    //Is the research available to be unlocked?
    //Needs credits, previous research and high enough population
    public bool CanBeUnlocked()
    {
        return GameManager.Instance.researchCredits > cost && DependencyUnlocked() && GameManager.Instance.totalPopulation >= popNeeded;
    }

    //Check if the previous dependency has been successfully unlocked
    public bool DependencyUnlocked()
    {
        for (int i = 0; i < dependencies.Length; i++)
        {
            if (!ResearchManager.Instance.research[dependencies[i].researchName])
            {
                return false;
            }
        }
        return (dependencies != null);
    }

    //Called when the research window is opened
    public void OpenWindow()
    {
        popupIcon.sprite = icon;
        CanvasGroup cg = unlockButton.transform.parent.gameObject.GetComponent<CanvasGroup>();
        cg.alpha = 1;
        cg.blocksRaycasts = true;
        cg.interactable = true;

        //Pass the info about the currently selected research to the details+confirmation window
        unlockButton.unlockButton.onClick.RemoveAllListeners();
        unlockButton.unlockButton.onClick.AddListener(Unlock);

        unlockButton.PassInfo(this);
    }
}
