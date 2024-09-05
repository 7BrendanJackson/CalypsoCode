using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventClimate2 : MonoBehaviour
{
    [SerializeField] GameObject goal;
    [SerializeField] CanvasGroup pollutionMeter;
    [SerializeField] GameObject hiddenResearch, removeResearch,removeSpecialNode;
    int gameSpeed;
    private void Start()
    {
        gameSpeed = GameManager.Instance.gameSpeed;
        GameManager.Instance.SetGameSpeed(0);
        BuildingSystem.Instance.SetSelectingBuildings(false);
    }

    public void OnClick()
    {
        GameManager.Instance.SetDiscovery(true);
        pollutionMeter.alpha = 1;
        goal.SetActive(true);
        hiddenResearch.SetActive(true);
        removeResearch.SetActive(false);
        removeSpecialNode.SetActive(false);
        GameManager.Instance.SetGameSpeed(gameSpeed);
        GameMenu.Instance.RefreshHUD();
        BuildingSystem.Instance.SetSelectingBuildings(true);
    }
}
