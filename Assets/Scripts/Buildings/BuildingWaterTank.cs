using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Unused Currently
public class BuildingWaterTank : BuildingMaster
{
    public override void Initialize()
    {

        GameManager.Instance.RegisterWaterTank(this);
        TryGetComponent<AudioSource>(out AudioSource audioSource);

        if (audioSource != null) audioSource.Play();
    }
}
