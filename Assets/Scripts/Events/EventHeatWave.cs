using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHeatWave : MonoBehaviour
{
    [SerializeField] EventModifier sick, power;
    int gameSpeed;
    private void Start()
    {
        gameSpeed = GameManager.Instance.gameSpeed;
        GameManager.Instance.SetGameSpeed(0);
        BuildingSystem.Instance.SetSelectingBuildings(false);
    }

    public void OnClick()
    {
        GameManager.Instance.AddSickPopsModifier(sick);
        GameManager.Instance.AddNewPowerModifier(power);
        GameManager.Instance.SetGameSpeed(gameSpeed);
        GameMenu.Instance.RefreshHUD();
        BuildingSystem.Instance.SetSelectingBuildings(true);
    }
}
