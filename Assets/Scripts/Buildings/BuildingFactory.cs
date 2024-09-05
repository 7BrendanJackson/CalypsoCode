using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingFactory : BuildingMaster
{
    //Register the factory with the game manager
    public override void Initialize()
    {
        GameManager.Instance.RegisterFactory(this);
    }
}
