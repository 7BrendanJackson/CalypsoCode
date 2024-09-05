using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPowerPlant : BuildingMaster
{
    public override void Initialize()
    {
        GameManager.Instance.RegisterPowerPlant(this);
        TryGetComponent<AudioSource>(out AudioSource audioSource);
        if (audioSource != null) audioSource.Play();
    }
}
