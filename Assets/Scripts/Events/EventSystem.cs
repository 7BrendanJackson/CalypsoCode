using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventSystem : MonoBehaviour
{
    public static EventSystem Instance;
    private void Awake()
    {
        Instance = this;
    }

    //public void AddSickPops(int num, int num2)
    //{
    //    GameManager.Instance.forcedSickPops += num;
    //}

    //public void LoseWater(int num)
    //{
    //    GameManager.Instance.currentWaterStored -= num;
    //}

    //public void LoseHappiness(EventModifier em)
    //{
    //    GameManager.Instance.happinessModifiers.Add(em);
    //}

    //public void IncreaseWaterConsumption(string name, int num, int time)
    //{
    //    GameManager.Instance.waterModifiers.Add(new EventModifier(name, num, time));
    //}

    //public void IncreaseFoodConsumption(string name, int num, int time)
    //{
    //    GameManager.Instance.foodModifiers.Add(new EventModifier(name, num, time));
    //}

    //public void IncreasePowerConsumption(string name, int num, int time)
    //{
    //    GameManager.Instance.powerModifiers.Add(new EventModifier(name, num, time));
    //}

    //public void DisableFactories(string name, int num, int time)
    //{
    //    GameManager.Instance.disabledFactories.Add(new EventModifier(name, num, time));
    //}
}