using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Debug menu for testing and cheats
public class Cheatster : MonoBehaviour
{

    GameManager gm;
    GameMenu hud;

    public GameObject heatwave1, heatwave2;

    void Start()
    {
        gm = GameManager.Instance;
        hud = GameMenu.Instance;
    }

    void LogsAdd()
    {
        gm.currentLogsStored += 100;
        hud.RefreshHUD();
    }

    void FoodAdd()
    {
        gm.currentFoodStored += 100;
        hud.RefreshHUD();
    }

    void WaterAdd()
    {
        gm.currentWaterStored += 100;
        hud.RefreshHUD();
    }

    void BuildMatsAdd()
    {
        gm.currentStoredBuildingMaterials += 100;
        hud.RefreshHUD();
    }

    void ResearchCredsAdd()
    {
        gm.researchCredits += 100;
        hud.RefreshHUD();
    }

    void TriggerHeatWave()
    {
        if (heatwave1 != null) heatwave1.SetActive(true);
    }

    void TriggerHeatWave2()
    {
        if (heatwave2 != null) heatwave2.SetActive(true);
    }

    void AddClimate()
    {
        gm.climateLevel += 100;
        hud.RefreshHUD();
    }

    void CreditsAdd()
    {
        gm.credits += 1000;
        hud.RefreshHUD();
    }
}
