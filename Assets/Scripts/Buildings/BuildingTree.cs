using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The forest tiles that occupy the island
public class BuildingTree : BuildingMaster
{
    [SerializeField] int 
        droneReduction, 
        loggingReduction;

    [SerializeField] float selectiveLoggingIncome;
    public bool isBurnt;
    [SerializeField] MeshRenderer[] mr;
    [SerializeField] Material burntMat;

    public override void Initialize()
    {
        GameManager.Instance.RegisterTree(this);
        InstallUpgrades();
    }

    //Triggered by Fire Event
    public void SetBurnt()
    {
        isBurnt = true;

        //Make resource collection worthless
        logsRefund = 0;
        climateImpact = 0;

        //Change to burnt material
        for (int i = 0; i < mr.Length; i++)
        {
            mr[i].material = burntMat;
        }
    }

    //Install research upgrades to the building
    public void InstallUpgrades()
    {
        if (ResearchManager.Instance.research["Tree_Drone"])
        {
            buildTime -= droneReduction;
        }
        if (ResearchManager.Instance.research["Tree_LoggingSpeed"])
        {
            deconstructTime --;
        }
        if (ResearchManager.Instance.research["Tree_SelectiveLogging"])
        {
            //logsImpact += selectiveLoggingIncome;//1 per turn is too much
        }
    }
}
