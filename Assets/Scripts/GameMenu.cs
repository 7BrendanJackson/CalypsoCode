using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

//Handles pretty much all the HUD/UI elements
public class GameMenu : MonoBehaviour
{
    public static GameMenu Instance;
    GameManager gm;
    BuildingSystem bs;
    ResearchManager rm;
    Controls controls;
    [SerializeField] Sprite lockedSprite, windDisabled, solarDisabled, researchDisabled;

    [SerializeField] bool buildMenuOpen, researchMenuOpen;
    [SerializeField] CanvasGroup buildMenu, researchMenu, pollutionBar;


    [SerializeField] Button buildMenuResearchLabButton, buildMenuHouseButton, buildMenuCoalPlantButton, buildMenuFactoryButton, buildMenuFarmButton, buildMenuOfficesButton, buildMenuTreesButton, buildMenuResearchCentreButton, buildMenuSolarPlantButton, buildMenuWindTurbineButton, buildMenuCattleButton;

    //HUD
    [SerializeField] Color availableColor, unavailableColor;
    [SerializeField] Sprite lockedImage;
    [SerializeField] RectTransform timeBar, timebarTarget, timebarStart;
    [SerializeField] RectTransform pollutionStart, pollutionTarget, pollution;
    [SerializeField] TMP_Text pollutionNum;


    private void Awake()
    {
        Instance = this;
        controls = new Controls();
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.GameMenu.BuildMenu.performed += _ => ToggleBuildMenu();
        controls.GameMenu.ResearchMenu.performed += _ => ToggleResearchMenu();
    }

    private void OnDisable()
    {
        controls.Disable();
        controls.GameMenu.BuildMenu.performed -= _ => ToggleBuildMenu();
        controls.GameMenu.ResearchMenu.performed -= _ => ToggleResearchMenu();
    }

    private void Start()
    {
        gm = GameManager.Instance;
        bs = BuildingSystem.Instance;
        rm = ResearchManager.Instance;
        Invoke("RefreshHUD", .5f);
    }

    public bool specialNode;
    private void Update()
    {
        if (specialNode == false && rm.research["Special"])
        {
            FindObjectOfType<EventSpawner>().SpawnClimateChange2();
            specialNode = true;
        }
        gm.climateLevel = gm.CalcClimateImpact();
        pollutionNum.text = gm.climateLevel.ToString();
        timeBar.position = new Vector2(Mathf.Lerp(timebarStart.position.x, timebarTarget.position.x, gm.turnTimeCurrent / gm.turnTimeMax), timeBar.transform.position.y);

        pollution.position = new Vector2(Mathf.Lerp(pollutionStart.position.x, pollutionTarget.position.x, (float)gm.climateLevel / (float)gm.climateLevelMax), pollution.transform.position.y);
    }

    public void ToggleBuildMenu()
    {
        buildMenuOpen = !buildMenuOpen;
        buildMenu.alpha = buildMenuOpen ? 1 : 0;
        buildMenu.blocksRaycasts = buildMenuOpen;
        buildMenu.interactable = buildMenuOpen;

        RefreshBuildMenu();

    }

    [Header("House")]
    [Space(50)]
    [SerializeField] TMP_Text houseCreditCostText;
    [SerializeField] TMP_Text houseMatCostText, houseNameText, houseTimeText;

    [Header("Coal Plant")]
    [SerializeField] TMP_Text coalPlantCreditCostText;
    [SerializeField] TMP_Text coalPlantMatCostText, coalPlantNameText, coalPlantTimeText;

    [Header("Factory")]
    [SerializeField] TMP_Text factoryCreditCostText;
    [SerializeField] TMP_Text factoryMatCostText, factoryNameText, factoryTimeText;

    [Header("Farm")]
    [SerializeField] TMP_Text farmMatCostText;
    [SerializeField] TMP_Text farmNameText, farmTimeText;

    [Header("Research Centre")]
    [SerializeField] TMP_Text researchCentreCostText;
    [SerializeField] TMP_Text researchCentreNameText, researchCentreTimeText;
    [SerializeField] GameObject researchCentrePopsNeeded, researchCentreMax;

    [Header("Solar Plant")]
    [SerializeField] TMP_Text solarCostText;
    [SerializeField] TMP_Text solarNameText, solarTimeText;

    [Header("Wind Turbine")]
    [SerializeField] TMP_Text windCostText;
    [SerializeField] TMP_Text windNameText, windTimeText;

    [Header("Cattle Farm")]
    [SerializeField] TMP_Text cattleCostText;
    [SerializeField] TMP_Text cattleNameText, cattleTimeText;

    [ContextMenu("Force Refresh")]
    //Refresh the entire build menu with unlocks and material cost
    public void RefreshBuildMenu()
    {
        bool hasMats;

        //House
        houseMatCostText.text = bs.houseMaterialCostCurrent.ToString();
        houseTimeText.text = bs.houseBuildTimeCurrent.ToString();

        hasMats = gm.currentStoredBuildingMaterials >= bs.houseMaterialCostCurrent;

        buildMenuHouseButton.interactable = hasMats;
        //houseCreditCostText.color = hasCredits ? availableColor : unavailableColor;
        houseMatCostText.color = hasMats ? availableColor : unavailableColor;

        //CoalPlant
        coalPlantMatCostText.text = bs.coalPlantMaterialCostCurrent.ToString();
        //coalPlantCreditCostText.text = bs.coalPlantCreditCostCurrent.ToString();
        coalPlantTimeText.text = bs.coalPlantBuildTimeCurrent.ToString();

        //hasCredits = gm.credits >= bs.houseCreditCostCurrent;
        hasMats = gm.currentStoredBuildingMaterials >= bs.coalPlantMaterialCostCurrent;

        buildMenuCoalPlantButton.interactable = hasMats;
        //coalPlantCreditCostText.color = hasCredits ? availableColor : unavailableColor;
        coalPlantMatCostText.color = hasMats ? availableColor : unavailableColor;

        //Factory
        factoryMatCostText.text = bs.factoryMaterialCostCurrent.ToString();
        // factoryCreditCostText.text = bs.factoryCreditCostCurrent.ToString();
        factoryTimeText.text = bs.factoryBuildTimeCurrent.ToString();

        //hasCredits = gm.credits >= bs.factoryCreditCostCurrent;
        hasMats = gm.currentStoredBuildingMaterials >= bs.factoryMaterialCostCurrent;

        buildMenuFactoryButton.interactable = hasMats;
        //factoryCreditCostText.color = hasCredits ? availableColor : unavailableColor;
        factoryMatCostText.color = hasMats ? availableColor : unavailableColor;

        //buildMenuResearchLabButton.interactable = (ResearchManager.Instance.research["Building_ResearchLab"]);

        //Farm
        farmMatCostText.text = bs.farmMaterialCostCurrent.ToString();
        farmTimeText.text = bs.farmBuildTimeCurrent.ToString();
        hasMats = gm.currentStoredBuildingMaterials >= bs.farmMaterialCostCurrent;
        buildMenuFarmButton.interactable = hasMats;
        farmMatCostText.color = hasMats ? availableColor : unavailableColor;

        //Research Centre
        researchCentreMax.SetActive(gm.researchBuilding != null);
        researchCentrePopsNeeded.SetActive(gm.totalPopulation < 30);
        if (gm.totalPopulation >= 30)
        {
            researchCentreCostText.text = bs.researchCentreMaterialCostCurrent.ToString();
            researchCentreNameText.text = "Research Centre";
            hasMats = gm.currentStoredBuildingMaterials >= bs.researchCentreMaterialCostCurrent;
            buildMenuResearchCentreButton.interactable = hasMats && gm.researchBuilding == null;
            researchCentreCostText.color = hasMats ? availableColor : unavailableColor;
            researchCentreTimeText.color = Color.white;
            researchCentreTimeText.text = bs.researchCentreBuildTimeCurrent.ToString();
            SpriteState t = buildMenuResearchCentreButton.spriteState;
            t.disabledSprite = researchDisabled;
            buildMenuResearchCentreButton.spriteState = t;
        }
        else
        {
            researchCentreCostText.text = "?";
            researchCentreNameText.text = "Locked";
            researchCentreCostText.color = unavailableColor;
            buildMenuResearchCentreButton.interactable = false;
            researchCentreTimeText.text = "?";
            researchCentreTimeText.color = unavailableColor;
            SpriteState t = buildMenuResearchCentreButton.spriteState;
            t.disabledSprite = lockedSprite;
            buildMenuResearchCentreButton.spriteState = t;
        }

        //Solar
        if (rm.research["Building_Solar"])
        {
            solarCostText.text = bs.solarMaterialCostCurrent.ToString();
            solarTimeText.text = bs.solarBuildTimeCurrent.ToString();
            solarNameText.text = "Solar Plant";
            hasMats = gm.currentStoredBuildingMaterials >= bs.solarMaterialCostCurrent;
            buildMenuSolarPlantButton.interactable = hasMats;
            solarTimeText.color = Color.white;
            solarCostText.color = hasMats ? availableColor : unavailableColor;
            SpriteState t = buildMenuSolarPlantButton.spriteState;
            t.disabledSprite = windDisabled;
            buildMenuSolarPlantButton.spriteState = t;
        }
        else
        {
            solarCostText.text = "?";
            solarCostText.color = unavailableColor;
            solarTimeText.text = "?";
            solarTimeText.color = unavailableColor;
            solarNameText.text = "Locked";
            buildMenuSolarPlantButton.interactable = false;
            SpriteState t = buildMenuSolarPlantButton.spriteState;
            t.disabledSprite = lockedSprite;
            buildMenuSolarPlantButton.spriteState = t;
        }

        //Wind
        if (rm.research["Building_Wind"])
        {
            windCostText.text = bs.windMaterialCostCurrent.ToString();
            hasMats = gm.currentStoredBuildingMaterials >= bs.windMaterialCostCurrent;
            windCostText.color = hasMats ? availableColor : unavailableColor;
            buildMenuWindTurbineButton.interactable = hasMats;
            windTimeText.text = bs.windMaterialBuildTimeCurrent.ToString();
            windTimeText.color = availableColor;
            windNameText.text = "Wind Turbine";
            SpriteState t = buildMenuWindTurbineButton.spriteState;
            t.disabledSprite = windDisabled;
            buildMenuWindTurbineButton.spriteState = t;
        }
        else
        {
            windCostText.text = "?";
            windCostText.color = unavailableColor;
            windTimeText.text = "?";
            windTimeText.color = unavailableColor;
            windNameText.text = "Locked";
            buildMenuWindTurbineButton.interactable = false;
            SpriteState t = buildMenuWindTurbineButton.spriteState;
            t.disabledSprite = lockedImage;
            buildMenuWindTurbineButton.spriteState = t;
        }


        //Cattle
        cattleCostText.text = bs.cattleMaterialCostCurrent.ToString();
        cattleTimeText.text = bs.cattleBuildTimeCurrent.ToString();
        hasMats = gm.currentStoredBuildingMaterials >= bs.cattleMaterialCostCurrent;
        buildMenuCattleButton.interactable = hasMats;
        cattleCostText.color = hasMats ? availableColor : unavailableColor;

    }

    public void ToggleResearchMenu()
    {
        ResearchManager.Instance.UpdateResearchNodes();
        researchMenuOpen = !researchMenuOpen;
        researchMenu.alpha = researchMenuOpen ? 1 : 0;
        researchMenu.blocksRaycasts = researchMenuOpen;
        researchMenu.interactable = researchMenuOpen;
    }

    [SerializeField] TMP_Text totalPopsText, happinessText, foodText, waterText, researchCreditsText, buildingMatsText, monthText, powerText, logsText, creditsText;

    [Header("Popups"), Space]
    //Popouts
    [SerializeField] TMP_Text logsIncomePopup;
    [SerializeField] TMP_Text logsUsagePopup, logsStoredPopup;
    [SerializeField] TMP_Text popsTotalPopup, popsSickPopup;
    [SerializeField] TMP_Text foodIncomePopup, foodUsagePopup;
    [SerializeField] TMP_Text buildMatsIncomePopup, buildMatsStoredPopup;
    [SerializeField] TMP_Text researchCreditsIncomePopup, researchCreditsStoredPopup;
    [SerializeField] TMP_Text powerIncomePopup, powerUsagePopup;

    public void RefreshHUD()
    {
        pollutionBar.alpha = gm.discoveredClimateChange ? 1 : 0;
        totalPopsText.text = gm.totalPopulation.ToString();
        happinessText.text = gm.happiness.ToString();
        int t = gm.CalcFoodIncome() + gm.CalcFoodUsage();
        foodText.text = t.ToString();
        foodText.color = t < 0 ? unavailableColor : Color.green;
        waterText.text = gm.currentWaterStored.ToString();
        researchCreditsText.text = gm.researchCredits.ToString();
        buildingMatsText.text = gm.currentStoredBuildingMaterials.ToString();
        creditsText.text = gm.credits.ToString();
        powerText.text = gm.CalcEnergy().ToString();
        logsText.text = gm.currentLogsStored.ToString();
        string month = (gm.currentTurn % 12) switch
        {
            0 => "January",
            1 => "February",
            2 => "March",
            3 => "April",
            4 => "May",
            5 => "June",
            6 => "July",
            7 => "August",
            8 => "September",
            9 => "October",
            10 => "November",
            11 => "December",
            _ => "Unknown",
        };
        monthText.text = $"Month: {month}";

        //Popups
        logsIncomePopup.text = gm.CalcLogImpact().ToString();
        logsUsagePopup.text = gm.CalculateFactoryProduction().ToString();
        logsStoredPopup.text = gm.currentLogsStored.ToString();

        popsTotalPopup.text = gm.totalPopulation.ToString();
        popsSickPopup.text = gm.CalcSickPops().ToString();

        foodIncomePopup.text = gm.CalcFoodIncome().ToString();
        foodUsagePopup.text = gm.CalcFoodUsage().ToString();

        buildMatsIncomePopup.text = gm.CalculateFactoryProduction().ToString();
        buildMatsStoredPopup.text = gm.currentStoredBuildingMaterials.ToString();

        researchCreditsIncomePopup.text = gm.CalcResearchCreditsGain().ToString();
        researchCreditsStoredPopup.text = gm.researchCredits.ToString();

        powerIncomePopup.text = gm.EnergyIncome().ToString();
        powerUsagePopup.text = gm.EnergyUsage().ToString();
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SetGameSpeed(float speed)
    {
        Time.timeScale = speed;
    }
}
