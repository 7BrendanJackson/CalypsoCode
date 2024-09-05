using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Stores and manages a ton of gameplay related stuff like calculating resources, turns, buildings and events
[System.Serializable]

//Used by all Events and can easily build new ones in the inspector
public class EventModifier
{
    public string name;
    public string description;
    [Tooltip("Num = Resource or Intensity value \n Time = Turns/Months")]

    public int
        num,
        time;
}

public class GameManager : MonoBehaviour
{
    [SerializeField] EventSpawner es;
    public Controls controls;
    public string
        islandName,
        popsName;
    [Space(20)]

    public int gameSpeed;
    public float
        turnTimeMax,
        turnTimeCurrent;

    public int currentTurn;
    [Space(20)]

    public int totalPopulation;
    public static GameManager Instance;
    public int climateLevel;
    public int climateLevelMax;
    public bool discoveredClimateChange;
    [Space(20)]

    public BuildingResearch researchBuilding;    //Not a list since it is a unique building

    public List<BuildingHouse> houses;
    public List<BuildingFarm> farms;
    public List<BuildingWaterTank> waterTanks;
    public List<BuildingPowerPlant> powerPlants;
    public List<BuildingRecreation> recreations;
    public List<BuildingFactory> factories;
    public List<BuildingMaster> deconstructions;
    public List<BuildingTree> trees;
    public List<BuildingTree> loggingTrees;

    public List<EventModifier>
        happinessModifiers,
        waterModifiers,
        foodModifiers,
        powerModifiers,
        factoryModifiers,
        sickModifiers;

    [Space(20f)]
    public Grid gridLayout;
    public AudioSource uiAudioSource;
    public AudioClip audioPause, audioSetGameSpeed;
    public Image[] imageGameSpeed;

    //[Header("Resources")]
    //Can't use Header or space before this type of declaration or the layout messes up
    public int currentFoodStored,
        currentFoodProduction,
        currentWaterStored,
        currentLogsStored,
        currentWaterProduction,
        currentStoredBuildingMaterials,
        energy,
        happiness,
        sickPops,
        researchCredits,
        forcedSickPops,
        credits;


    //The official discovery of climate change triggered by special in-game event
    public void SetDiscovery(bool value)
    {
        discoveredClimateChange = value;

        //Update HUD to show new climate change bar
        GameMenu.Instance.RefreshHUD();
    }

    //Register any event with the game manager
    //So it can proceed to act on them each turn
    public void AddNewHappinessModifier(EventModifier em)
    {
        happinessModifiers.Add(em);
    }
    public void AddNewWaterModifier(EventModifier em)
    {
        waterModifiers.Add(em);
    }
    public void AddNewFoodModifier(EventModifier em)
    {
        foodModifiers.Add(em);
    }
    public void AddNewPowerModifier(EventModifier em)
    {
        powerModifiers.Add(em);
    }
    public void AddNewFactoryModifier(EventModifier em)
    {
        factoryModifiers.Add(em);
    }
    public void AddSickPopsModifier(EventModifier em)
    {
        sickModifiers.Add(em);
    }

    //Passed through when clicking start on the main menu
    public void InputPopsName(string name)
    {
        popsName = name;
    }
    //Same as above
    public void InputIslandName(string name)
    {
        islandName = name;
    }

    private void Awake()
    {
        Instance = this;
        controls = new Controls();
    }

    //Bind  1, 2, 3 and space to time speed
    private void OnEnable()
    {
        controls.Enable();
        controls.Time.Pause.performed += _ => SetGameSpeed(0);
        controls.Time._1x.performed += _ => SetGameSpeed(1);
        controls.Time._2x.performed += _ => SetGameSpeed(2);
        controls.Time._3x.performed += _ => SetGameSpeed(3);
    }

    //Unbind controls
    private void OnDisable()
    {
        controls.Disable();
        controls.Time.Pause.performed -= _ => SetGameSpeed(0);
        controls.Time._1x.performed -= _ => SetGameSpeed(1);
        controls.Time._2x.performed -= _ => SetGameSpeed(2);
        controls.Time._3x.performed -= _ => SetGameSpeed(3);
    }

    private void Start()
    {
        //Immediately calculate population since game starts with 2 houses pre-built
        totalPopulation = CalcPopulation();
    }

    private void Update()
    {
        //Advance time based on gamespeed
        turnTimeCurrent += gameSpeed * Time.deltaTime;

        //Goto next month (called 'turns' in code)
        for (int i = 0; turnTimeCurrent >= turnTimeMax; i++)
        {
            turnTimeCurrent -= turnTimeMax;
            NextTurn();
        }
    }

    [ContextMenu("Advance Turn")]
    public void NextTurn()
    {
        currentTurn++;

        //Update all resources
        LogTrees();
        SetFood();
        SetPopulation();
        SetEnergy();
        SetWater();
        SetHappiness();
        SetLogs();
        SetResearchCredits();

        //Deconstruct pending buildings by 1 turn
        DeconstructBuildings();

        //Reduce current event timers by 1
        ReduceEvents();

        //Do any events need to be triggered?
        es.CheckToSpawn();

        //Update pending research
        ResearchManager.Instance.updateNodes.Invoke();

        //Update UI
        GameMenu.Instance.RefreshBuildMenu();
        GameMenu.Instance.RefreshHUD();
    }

    //Reduce the counter for all currently running events
    //Loop backwards to remove finished events
    void ReduceEvents()
    {
        //Happiness
        for (int i = happinessModifiers.Count - 1; i >= 0; i--)
        {
            happinessModifiers[i].time--;
            if (happinessModifiers[i].time <= 0) happinessModifiers.RemoveAt(i);
        }
        //Food
        for (int i = foodModifiers.Count - 1; i >= 0; i--)
        {
            foodModifiers[i].time--;
            if (foodModifiers[i].time <= 0) foodModifiers.RemoveAt(i);
        }
        //Water
        for (int i = waterModifiers.Count - 1; i >= 0; i--)
        {
            waterModifiers[i].time--;
            if (waterModifiers[i].time <= 0) waterModifiers.RemoveAt(i);
        }
        //Power (electricity)
        for (int i = powerModifiers.Count - 1; i >= 0; i--)
        {
            powerModifiers[i].time--;
            if (powerModifiers[i].time <= 0) powerModifiers.RemoveAt(i);
        }
        //Factories
        for (int i = factoryModifiers.Count - 1; i >= 0; i--)
        {
            factoryModifiers[i].time--;
            if (factoryModifiers[i].time <= 0) factoryModifiers.RemoveAt(i);
        }
        //Sick pops
        for (int i = sickModifiers.Count - 1; i >= 0; i--)
        {
            sickModifiers[i].time--;
            if (sickModifiers[i].time <= 0) sickModifiers.RemoveAt(i);
        }
    }

    //All buildings register here
    #region
    public void RegisterHouse(BuildingHouse house)
    {
        houses.Add(house);
        CalcPopulation();
    }

    public void RegisterResearchBuilding(BuildingResearch research)
    {
        researchBuilding = research;
    }

    public void RegisterFarm(BuildingFarm farm)
    {
        farms.Add(farm);
        CalcFoodImpact();
    }

    public void RegisterFactory(BuildingFactory factory)
    {
        factories.Add(factory);
    }

    public void RegisterTree(BuildingTree tree)
    {
        trees.Add(tree);
    }

    public void RegisterPowerPlant(BuildingPowerPlant powerplant)
    {
        powerPlants.Add(powerplant);
    }
    #endregion

    //Called when a building has been marked for deconstruction
    public void RegisterForDeconstruction(BuildingMaster target)
    {

        if (!deconstructions.Contains(target)) //Start deconstruct
        {
            if (target.deconstructSprite != null) target.deconstructSprite.enabled = true;
            deconstructions.Add(target);
            if (target.deconstructSprite != null) target.isDeconstructing = true;
            if (target.counterText != null)
            {
                target.counterText.enabled = true;
                target.counterText.text = target.deconstructionCurrent.ToString();
            }
        }
        else //Cancel deconstruct
        {
            if (target.deconstructSprite != null) target.deconstructSprite.enabled = false;
            deconstructions.Remove(target);
            if (target.deconstructSprite != null) target.isDeconstructing = false;
            if (target.counterText != null) target.counterText.enabled = false;
            target.deconstructionCurrent = target.deconstructTime;
        }
    }

    public void RegisterForLogging(BuildingTree target)
    {
        //Check if trees are currently being deconstructed already
        if (!loggingTrees.Contains(target)) //Start Logging
        {
            target.deconstructSprite.enabled = true;
            target.counterText.enabled = true;
            loggingTrees.Add(target);

        }
        else //Cancel Loggin
        {
            loggingTrees.Remove(target);
            target.deconstructSprite.enabled = false;
            target.counterText.enabled = false;
        }
        CalcTreeLoggingTimes();
    }

    public void LogTrees()
    {
        //Disabled for balancing testing
        //int amount = ResearchManager.Instance.research["Tree_LoggingSpeed"] ? 2 : 1;

        //Lower the removal timer for trees based on research
        if (loggingTrees.Count > 0)
        {
            loggingTrees[0].deconstructionCurrent--;
            //If has fast logging research, reduce by a total of 2 turns each turn
            if (ResearchManager.Instance.research["Tree_LoggingMultiple"] && loggingTrees.Count > 1)
            {
                loggingTrees[1].deconstructionCurrent--;
            }
        }

        for (int i = loggingTrees.Count - 1; i >= 0; i--)
        {
            if (loggingTrees[i].deconstructionCurrent <= 0)
            {
                int t = loggingTrees[i].logsRefund;
                currentLogsStored += t;

                //Credits are currently a cut feature
                credits += t * 100;
                BuildingMaster tt = loggingTrees[i];

                //Placer contains the information about grid size, position and rotation etc.
                PlaceableObject placer = loggingTrees[i].GetComponent<PlaceableObject>();

                //Clear the grid where the trees once stood (according to the placer info)
                BuildingSystem.Instance.ClearArea(gridLayout.WorldToCell(placer.GetStartPosition()), placer.Size);

                //Delete the tree from existance
                loggingTrees.Remove(loggingTrees[i]);
                Destroy(tt.gameObject);
            }
        }
        CalcTreeLoggingTimes();
    }

    //Since loggings trees stacks over time it needs to be calculated differently
    public void CalcTreeLoggingTimes()
    {
        int num = 0;
        if (!ResearchManager.Instance.research["Tree_LoggingMultiple"]) //If mutli tree logging has not been researched (Log 2 trees each turn)
        {
            for (int i = 0; i < loggingTrees.Count; i++)
            {
                num += loggingTrees[i].deconstructionCurrent;
                loggingTrees[i].counterText.text = num.ToString();
            }
        }
        else //If it is researched
        {
            for (int i = 0; i < loggingTrees.Count; i++)
            {
                if (i % 2 == 0) num += loggingTrees[i].deconstructionCurrent;
                loggingTrees[i].counterText.text = num.ToString();
            }
        }
    }

    //Triggered by the forest fire event
    //Calculates a randomish amount of trees to burn based on severity
    [ContextMenu("Burn Trees")]
    public void BurnTrees(float num)
    {
        int t = Mathf.FloorToInt(trees.Count * num);
        int n;
        for (int i = t; i > 0; i--)
        {
            n = Random.Range(0, trees.Count);
            if (!trees[n].isBurnt)
            {
                trees[n].SetBurnt();
            }
        }
    }

    //Triggered by crop death event 
    //Bypasses and force removes a set amount of stored food
    public void ModFarmFood(int num)
    {
        for (int i = 0; i < farms.Count; i++)
        {
            farms[i].foodImpact -= num;
        }
        BuildingSystem.Instance.farmFoodImpactCurrrent -= num;
    }

    [ContextMenu("Deconstruct Buildings")]
    void DeconstructBuildings()
    {
        //Might need to filter through backwards if there are issues?
        for (int i = 0; i < deconstructions.Count; i++)
        {
            deconstructions[i].deconstructionCurrent--;
            if (deconstructions[i].counterText != null) deconstructions[i].counterText.text = deconstructions[i].deconstructionCurrent.ToString();
            if (deconstructions[i].deconstructionCurrent <= 0)
            {
                currentStoredBuildingMaterials += deconstructions[i].buildMatRefund;

                //Cleanup buildng and 
                BuildingMaster targetDeconstruct = deconstructions[i];
                PlaceableObject placer = deconstructions[i].GetComponent<PlaceableObject>();
                BuildingSystem.Instance.ClearArea(gridLayout.WorldToCell(placer.GetStartPosition()), placer.Size);
                deconstructions.Remove(deconstructions[i]);
                Destroy(targetDeconstruct.gameObject);
            }
        }
    }

    [ContextMenu("Calculate Build Mat Production")]
    public int CalculateFactoryProduction()
    {
        int disabled = 0;
        for (int i = 0; i < factoryModifiers.Count; i++)
        {
            disabled += factoryModifiers[i].num;
        }
        int t = 0;
        for (int i = 0; i < factories.Count - disabled; i++)
        {
            //if (factoryModifiers.Count > 0 && i! < factoryModifiers[0].num)
            t += factories[i].buildingMatsImpact;
        }
        t = Mathf.Clamp(t, 0, currentLogsStored);
        return t;
    }

    public int CalcLogImpact()
    {
        int t = 0;
        for (int i = 0; i < loggingTrees.Count; i++)
        {
            if (loggingTrees[i].deconstructionCurrent <= 1)
            {
                t += loggingTrees[i].logsImpact;
            }
        }
        return t;
    }

    //Gain logs from cutting down forest and then turn them into building materials based on factories
    void SetLogs()
    {
        currentLogsStored += CalcLogImpact();
        int t = CalculateFactoryProduction();

        currentStoredBuildingMaterials += t;
        currentLogsStored -= t;
    }

    //Gain research credits based on the amount of healthy pops
    //Sick pops do not contribute
    [ContextMenu("Force Calc Research Credits")]
    public int CalcResearchCreditsGain()
    {
        int t = Mathf.Clamp(Mathf.RoundToInt((CalcPopulation() - CalcSickPops()) * .1f), 0, 100);
        return t;
    }

    void SetResearchCredits()
    {
        researchCredits += CalcResearchCreditsGain();
    }



    [ContextMenu("Calaculate Food Production")]
    public int CalcFoodImpact()
    {
        int t = 0;
        for (int i = 0; i < farms.Count; i++)
        {
            t += farms[i].foodImpact;
        }
        for (int i = 0; i < houses.Count; i++)
        {
            t += houses[i].foodImpact; //Houses provide a small food offset if the home gardens is researched
        }
        return (t);
    }

    //Food drain of pops
    public int CalcFoodUsage()
    {
        int t = 0;
        for (int i = 0; i < houses.Count; i++)
        {
            t += houses[i].foodImpact;
        }
        return (t);
    }

    public int CalcFoodIncome()
    {
        int t = 0;
        for (int i = 0; i < farms.Count; i++)
        {
            t += farms[i].foodImpact;
        }
        for (int i = 0; i < foodModifiers.Count; i++)
        {
            t -= foodModifiers[i].num;
        }
        return Mathf.Abs(t);
    }

    void SetFood()
    {
        currentFoodStored += CalcFoodImpact();
    }

    //Calculate and return the current population of the island
    [ContextMenu("Calculate Population")]
    public int CalcPopulation()
    {
        int t = 0;
        for (int i = 0; i < houses.Count; i++)
        {
            t += houses[i].populationImpact;
        }
        return t;
    }

    //Updates the current stored population of the island
    void SetPopulation()
    {
        totalPopulation = CalcPopulation();
    }

    //Adds the power plant and factory emission output against the amount of trees
    [ContextMenu("Calculate Climate Impact")]
    public int CalcClimateImpact()
    {
        int t = 0;
        for (int i = 0; i < trees.Count; i++)
        {
            t += trees[i].climateImpact;
        }
        for (int i = 0; i < powerPlants.Count; i++)
        {
            t += powerPlants[i].climateImpact;
        }
        for (int i = 0; i < factories.Count; i++)
        {
            t += factories[i].climateImpact;
        }
        return t;
    }


    //Currently Water is cut content
    int CalcWaterImpact()
    {
        int t = 0;
        for (int i = 0; i < farms.Count; i++)
        {
            t += farms[i].waterImpact;
        }
        for (int i = 0; i < houses.Count; i++)
        {
            t += houses[i].waterImpact;
        }
        for (int i = 0; i < waterTanks.Count; i++)
        {
            t += waterTanks[i].waterImpact;
        }
        return t;
    }

    public void RegisterWaterTank(BuildingWaterTank waterTank)
    {
        waterTanks.Add(waterTank);
    }

    void SetWater()
    {
        currentWaterStored += CalcWaterImpact();
    }

    public int CalcEnergy()
    {
        int t = 0;
        for (int i = 0; i < powerPlants.Count; i++)
        {
            t += powerPlants[i].energyImpact;
        }
        for (int i = 0; i < farms.Count; i++)
        {
            t += farms[i].energyImpact;
        }
        for (int i = 0; i < houses.Count; i++)
        {
            t += houses[i].energyImpact;
        }
        for (int i = 0; i < factories.Count; i++)
        {
            t += factories[i].energyImpact;
        }

        if (researchBuilding) t += researchBuilding.energyImpact;
        return t;
    }

    //Calculate energy production of the island
    public int EnergyIncome()
    {
        int t = 0;
        for (int i = 0; i < powerPlants.Count; i++)
        {
            //I'm pretty sure it can only be a positive number
            //Not sure why the absolute is needed
            t += Mathf.Abs(powerPlants[i].energyImpact);
        }
        return t;
    }

    //Calculate energy usage for the island
    public int EnergyUsage()
    {
        int t = 0;
        for (int i = 0; i < farms.Count; i++)
        {
            t += farms[i].energyImpact;
        }
        for (int i = 0; i < houses.Count; i++)
        {
            t += houses[i].energyImpact;
        }
        for (int i = 0; i < factories.Count; i++)
        {
            t += factories[i].energyImpact;
        }

        //If the research building has been build calculate that amount
        //Since it's a unique building it can't be a list
        if (researchBuilding) t += researchBuilding.energyImpact;
        return t;
    }

    void SetEnergy()
    {
        energy = CalcEnergy();
    }
    //calc energy need
    //calc energy production
    //check if energy credits is researched
    [ContextMenu("Print CalcHappiness")]
    public void DebugCalcHappiness()
    {
        print(CalcHappiness());
    }

    int CalcHappiness()
    {
        int t = 100;
        //farms
        for (int i = 0; i < farms.Count; i++)
        {
            t += farms[i].happinessImpact;
        }
        //recreations
        for (int i = 0; i < recreations.Count; i++)
        {
            t += recreations[i].happinessImpact;
        }
        //events
        for (int i = 0; i < happinessModifiers.Count; i++)
        {
            t += happinessModifiers[i].num;
        }
        return Mathf.Clamp(t, 0, 200);
    }

    void SetHappiness()
    {
        happiness = CalcHappiness();
    }

    public int CalcSickPops()
    {
        int t = 0;
        for (int i = 0; i < sickModifiers.Count; i++)
        {
            t += sickModifiers[i].num;
        }
        return t;
    }

    public void SetGameSpeed(int num)
    {
        if (gameSpeed == num) return;
        for (int i = 0; i < imageGameSpeed.Length; i++)
        {
            imageGameSpeed[i].color = num == i ? Color.cyan : Color.white;
        }
        gameSpeed = num;

        //Change pitch based on speed selected
        uiAudioSource.pitch = 1 + (.6f * num);
        uiAudioSource.PlayOneShot(audioSetGameSpeed);
    }
}
