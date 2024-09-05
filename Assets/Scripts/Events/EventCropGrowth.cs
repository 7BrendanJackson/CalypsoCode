using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCropGrowth : MonoBehaviour
{
    [SerializeField] int food;
    int gameSpeed;
    private void Start()
    {
        gameSpeed = GameManager.Instance.gameSpeed;
        GameManager.Instance.SetGameSpeed(0);
        BuildingSystem.Instance.SetSelectingBuildings(false);
    }

    public void OnClick()
    {
        GameManager.Instance.ModFarmFood(food);

        GameManager.Instance.SetGameSpeed(gameSpeed);
        GameMenu.Instance.RefreshHUD();
        BuildingSystem.Instance.SetSelectingBuildings(true);
    }
}
