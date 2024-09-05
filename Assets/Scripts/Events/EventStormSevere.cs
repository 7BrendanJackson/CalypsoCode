using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventStormSevere : MonoBehaviour
{
    [SerializeField] EventModifier factory;
    [SerializeField] int logs, mats;
    int gameSpeed;
    private void Start()
    {
        //Pause and unselect current building
        gameSpeed = GameManager.Instance.gameSpeed;
        GameManager.Instance.SetGameSpeed(0);
        BuildingSystem.Instance.SetSelectingBuildings(false);
    }

    public void OnClick()
    {

        GameManager.Instance.factoryModifiers.Add(factory);

        //Reduce the player's logs
        GameManager.Instance.currentLogsStored -= logs;
        GameManager.Instance.currentLogsStored = Mathf.Clamp(GameManager.Instance.currentLogsStored, 0, 999);

        //Reduce the player's Building Materials
        GameManager.Instance.currentStoredBuildingMaterials -= mats;
        GameManager.Instance.currentStoredBuildingMaterials = Mathf.Clamp(GameManager.Instance.currentStoredBuildingMaterials, 0, 999);

        //Resume play
        GameManager.Instance.SetGameSpeed(gameSpeed);
        GameMenu.Instance.RefreshHUD();
        BuildingSystem.Instance.SetSelectingBuildings(true);
    }
}
