using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingResearch : BuildingMaster
{
    public override void Initialize()
    {
        GameManager.Instance.RegisterResearchBuilding(this);
    }
}
