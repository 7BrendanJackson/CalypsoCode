using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Manages all of the locked and unlocked research
public class ResearchManager : MonoBehaviour
{
    public Dictionary<string, bool> research = new Dictionary<string, bool>();
    public bool houseSolar, houseGarden, houseWaterTank, houseHaste, houseLowerCost;

    public bool forestDronePlanting, forestChainsaw;
    public UnityEvent updateNodes;

    public static ResearchManager Instance;

    private void Awake()
    {
        Instance = this;

        //Builds the list of all unlockable research
        Add("House_BuildHaste");//Build faster
        Add("House_Solar");//Reduce energy consumption
        Add("House_WaterTank");//Reduce water consumption
        Add("House_Garden");//Reduce food consumption
        Add("House_Insulation");//Lessen weather events
        Add("House_IncreasedCapacity");//Increased pop impact
        Add("House_ReducedCost");//Reduce material and credit cost

        Add("Tree_Drone");//Trees grow faster
        Add("Tree_LoggingSpeed");//Cut down trees faster
        Add("Tree_LoggingMultiple");//Cut down multple trees in a single turn
        Add("Tree_SelectiveLogging");//Slow but passive logs

        Add("Factory_ProduceHaste");//Produce more building materials
        Add("Factory_ReducedCost");//Cheaper construction cost
        Add("Factory_Build Haste");//Build Faster

        Add("Farm_CropRotation");//Increase crop yields
        Add("Farm_IrrigationEfficiency");//Farms use less water

        Add("PowerPlant_EnergyCredits");//Excess energy gets turned into money
        Add("PowerPlant_Overdrive");//Powerplant can burn fuel faster/output more

        Add("Solar_ImprovedCells");//Increased power production

        Add("Building_LabGrownMeat");
        Add("Building_Solar");
        //research["Building_Solar"] = true;
        Add("Building_Wind");
        Add("Special");
        //research["Building_Wind"] = true;
        Add("Building_Hyroponics");
        Add("Building_ResearchLab");

    }

    //Add new research to tech tree
    void Add(string name)
    {
        //Newly added research starts as disabled
        research.Add(name, false);
    }

    //Install upgrades on house automatically when researched
    //Only houses since they are the only thing that change appearance
    public void InstallHouseUpgrades()
    {
        //Upgrades all registered houses
        List<BuildingHouse> t = GameManager.Instance.houses;
        for (int i = 0; i < t.Count; i++)
        {
            t[i].InstallUpgrades();
        }
    }

    //Refresh all research nodes
    public void UpdateResearchNodes()
    {
        updateNodes?.Invoke();
    }
}
