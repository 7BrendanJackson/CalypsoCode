using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingFarm : BuildingMaster
{
    
    public override void Initialize()
    {
        //On build, register farm plot to the game manager and update food income
        GameManager.Instance.RegisterFarm(this);
        foodImpact = BuildingSystem.Instance.farmFoodImpactCurrrent;
        InstallUpgrades();
    }
    
    //Install researched upgrades into the farm plot
    void InstallUpgrades()
    {
        if (ResearchManager.Instance.research["Farm_CropRotation"])
        {
            foodImpact += 10;
        }
    }
}
