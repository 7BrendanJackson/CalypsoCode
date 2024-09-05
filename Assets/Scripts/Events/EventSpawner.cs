using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EventSpawner : MonoBehaviour
{
    [SerializeField] GameObject heatwave, heatwaveSevere, storm, stormSevere, cropGrowth, fire, fireSevere, researchUnlock, climateChange1, climateChange2, finale;
    bool heatwaveSpawned, heatwaveSevereSpawned, stormSpawned, stormSevereSpawned, cropGrowthSpawned, fireSpawned, fireSevereSpawned, researchUnlockSpawned, climateChange1Spawned, climateChange2Spawned, finaleSpawned;
    GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.Instance;
    }

    public void CheckToSpawn()
    {
        if (gm.currentTurn > 12 && gm.totalPopulation > 30 && !heatwaveSpawned)
        {
            heatwaveSpawned = true;
            heatwave.SetActive(true);
        }
        if (!researchUnlockSpawned && gm.CalcPopulation() >= 30)
        {
            researchUnlockSpawned = true;
            researchUnlock.SetActive(true);
        }
        if (!finaleSpawned && climateChange2Spawned && gm.climateLevel == 0)
        {
            finale.SetActive(true);
        }
    }

    public void SpawnClimateChange2()
    {
        if (climateChange2Spawned) return;
        climateChange2.SetActive(true);
        climateChange2Spawned = true;
    }
}
