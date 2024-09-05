using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The main building where all the pops live
public class BuildingHouse : BuildingMaster
{
    BuildingSystem bs;
    [SerializeField] GameObject 
        solar, 
        waterTank, 
        garden;

    [SerializeField] int 
        solarReduction, 
        waterTankReduction, 
        gardenReduction, 
        hasteReduction, 
        increasedCapacity;


    private void Awake()
    {
        bs = BuildingSystem.Instance;
        if (bs == null) return;
        buildCreditCost = bs.houseCreditCostCurrent;
        buildMatCost = bs.houseMaterialCostCurrent;
    }
    private void Start()
    {
        InstallUpgrades();
    }

    public override void Initialize()
    {

        GameManager.Instance.RegisterHouse(this);

        //Play placement sound if it has one
        TryGetComponent<AudioSource>(out AudioSource audioSource);
        if (audioSource) audioSource.Play();

        //Connect driveway to road
        CheckForRoadConnection();
        InstallUpgrades();
    }

    //Gets called by the building manager when placed
    public void InstallUpgrades()
    {
        
        //if HasUpgrade
        //Turn on cosmetic
        //Update the stats
        if (ResearchManager.Instance.research["House_Solar"])
        {
            solar.SetActive(true);
            energyImpact += solarReduction;
        }
        if (ResearchManager.Instance.research["House_WaterTank"])
        {
            waterTank.SetActive(true);
            waterImpact += waterTankReduction;
        }
        if (ResearchManager.Instance.research["House_Garden"])
        {
            garden.SetActive(true);
            foodImpact += gardenReduction;
        }

        if (ResearchManager.Instance.research["House_IncreasedCapacity"])
        {
            populationImpact += increasedCapacity;
        }
    }
}
