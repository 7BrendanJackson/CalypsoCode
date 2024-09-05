using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventStorm : MonoBehaviour
{
    [SerializeField] EventModifier factory;
    [SerializeField] int logs, mats;
    int gameSpeed;
    private void Start()
    {
        gameSpeed = GameManager.Instance.gameSpeed;
        GameManager.Instance.SetGameSpeed(0);
        BuildingSystem.Instance.SetSelectingBuildings(false);
    }

    public void OnClick()
    {
        GameManager.Instance.factoryModifiers.Add(factory);

        GameManager.Instance.currentLogsStored -= logs;
        GameManager.Instance.currentLogsStored = Mathf.Clamp(GameManager.Instance.currentLogsStored, 0, 999);

        GameManager.Instance.currentStoredBuildingMaterials -= mats;
        GameManager.Instance.currentStoredBuildingMaterials = Mathf.Clamp(GameManager.Instance.currentStoredBuildingMaterials, 0, 999);

        GameManager.Instance.SetGameSpeed(gameSpeed);
        GameMenu.Instance.RefreshHUD();
        BuildingSystem.Instance.SetSelectingBuildings(true);
    }
}
