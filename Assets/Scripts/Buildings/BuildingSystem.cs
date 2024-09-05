using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using System;

//Manages pretty much everything to do with buying and placing buildings on the grid
public class BuildingSystem : MonoBehaviour
{
    //Update Selected building on road place | This may not be an issue later since the build menu may remove the selected window
    [SerializeField] LayerMask floorMask, buildingMask;
    Camera mainCamera;
    [SerializeField] BuildingMaster selectedBuilding;
    [SerializeField] CanvasGroup selectedBuildingUIGroup;
    [SerializeField] float canvasFadeSpeed = 4;
    Controls controls;
    public static BuildingSystem Instance;
    public GridLayout gridLayout;
    Grid grid;
    [SerializeField] TilemapRenderer tr;
    public Tilemap MainTilemap;
    [SerializeField] TileBase buildingTile, roadTile;

    [SerializeField] GameObject[] buildingList;

    public List<Vector3Int> painted;
    public PlaceableObject objectToPlace;
    int replaceObject;
    [SerializeField] Transform buildingHolder;
    int placementRot;
    public bool canSelectBuildings = true;

    [SerializeField] TMP_Text 
        selectedBuildingName, 
        selectedBuildingEnergy, 
        selectedBuildingPopulation, 
        selectedBuildingFood, 
        selectedBuildingWater, 
        selectedBuildingHappiness, 
        selectedBuildingBuildingMats;

    [SerializeField] Toggle selectedBuildingRoadConnected;

    GameManager gm;
    ResearchManager rm;

    public void SetSelectingBuildings(bool input)
    {
        canSelectBuildings = input;
    }

    //Building Cost
    //Start = Base cost | Current = Cost with research and events taken into account
    //House
    [HideInInspector] public int 
        houseCreditCostCurrent, 
        houseMaterialCostCurrent, 
        houseBuildTimeCurrent;
    public int 
        houseCreditCostStart, 
        houseMaterialCostStart, 
        houseBuildTimeStart;

    //Coal Plant
    [HideInInspector] public int 
        coalPlantCreditCostCurrent, 
        coalPlantMaterialCostCurrent, 
        coalPlantBuildTimeCurrent;
    public int coalPlantCreditCostStart, 
        coalPlantMaterialCostStart, 
        coalPlantBuildTimeStart;

    //Factory
    [HideInInspector] public int 
        factoryCreditCostCurrent, 
        factoryMaterialCostCurrent, 
        factoryBuildTimeCurrent;
    public int 
        factoryCreditCostStart, 
        factoryMaterialCostStart, 
        factoryBuildTimeStart;

    //Farm
    public int 
        farmMaterialCostCurrent, 
        farmBuildTimeCurrent, 
        farmFoodImpactCurrrent = 100;
    public int 
        farmMaterialCostStart, 
        farmBuildTimeStart, 
        farmFoodImpactStart = 100;

    //Trees/Forest
    [HideInInspector] public int 
        treesBuildTimeCurrent;
    public int 
        treesBuildTimeStart;

    //Research Centre
    public int 
        researchCentreMaterialCostCurrent, 
        researchCentreBuildTimeCurrent;

    //Solar Farm
    public int 
        solarMaterialCostCurrent, 
        solarBuildTimeCurrent;

    //Wind Turbine
    public int 
        windMaterialCostCurrent, 
        windMaterialBuildTimeCurrent;

    //Cattle Farm
    public int 
        cattleMaterialCostCurrent, 
        cattleBuildTimeCurrent;

    [SerializeField]
    TMP_Text
        EnergyInfo,
        PopulationInfo;



    //Calculate all the current stats the buildings should have based on research and events
    void CalcAllBuildingStats()
    {
        CalcHouseStats();
        CalcCoalPlantStats();
        CalcFactoryStats();
        CalcFarmStats();
    }

     void CalcHouseStats()
    {
        houseCreditCostCurrent = houseCreditCostStart;
        houseMaterialCostCurrent = rm.research["House_ReducedCost"] ? houseMaterialCostStart - 1 : houseMaterialCostStart;
        houseBuildTimeCurrent = rm.research["House_BuildHaste"] ? houseBuildTimeStart - 1 : houseBuildTimeStart;
    }

     void CalcCoalPlantStats()
    {
        coalPlantCreditCostCurrent = coalPlantCreditCostStart;
        coalPlantMaterialCostCurrent = coalPlantMaterialCostStart;
        coalPlantBuildTimeCurrent = coalPlantBuildTimeStart;
    }

     void CalcFactoryStats()
    {
        factoryCreditCostCurrent = factoryCreditCostStart;
        factoryMaterialCostCurrent = factoryMaterialCostStart;
        factoryBuildTimeCurrent = factoryBuildTimeStart;
    }

     void CalcFarmStats()
    {
        farmMaterialCostCurrent = farmMaterialCostStart;
        farmBuildTimeCurrent = farmBuildTimeStart;
    }

    private void Awake()
    {
        //Init.
        Instance = this;
        controls = new Controls();
        grid = gridLayout.gameObject.GetComponent<Grid>();
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        gm = GameManager.Instance;
        rm = ResearchManager.Instance;
        CalcAllBuildingStats();
    }

    //Bind Controls
    private void OnEnable()
    {
        controls.Enable();
        controls.BuildingPlacement.Place.performed += _ => PlaceBuilding();
        controls.BuildingPlacement.Place.performed += _ => SelectBuilding();
        controls.BuildingPlacement.Cancel.performed += _ => CancelBuildingPlacement();
        controls.BuildingPlacement.Rotate.performed += _ => Rotate();
        controls.BuildingPlacement.Delete.performed += _ => MarkForDeconstruction();

        controls.Debug.DebugAction1.performed += _ => SpawnBuilding(0);
        controls.Debug.DebugAction2.performed += _ => SpawnBuilding(1);
        controls.Debug.DebugAction3.performed += _ => SpawnBuilding(2);
        controls.Debug.Advance.performed += _ => GameManager.Instance.NextTurn();
        //controls.Debug.Undo.performed += _ => UndoPaint();
        //controls.Debug.DebugAction4.performed += _ => ClearPaintTiles();
    }

    //Unbind Controls
    private void OnDisable()
    {
        controls.Disable();
        controls.BuildingPlacement.Place.performed -= _ => PlaceBuilding();
        controls.BuildingPlacement.Place.performed -= _ => SelectBuilding();
        controls.BuildingPlacement.Cancel.performed -= _ => CancelBuildingPlacement();
        controls.BuildingPlacement.Rotate.performed -= _ => Rotate();
        controls.BuildingPlacement.Delete.performed -= _ => MarkForDeconstruction();

        controls.Debug.DebugAction1.performed -= _ => SpawnBuilding(0);
        controls.Debug.DebugAction2.performed -= _ => SpawnBuilding(1);
        controls.Debug.DebugAction3.performed -= _ => SpawnBuilding(2);
        controls.Debug.Advance.performed -= _ => GameManager.Instance.NextTurn();
        //controls.Debug.DebugAction4.performed -= _ => ClearPaintTiles();
        //controls.Debug.Undo.performed -= _ => UndoPaint();
    }

    //Show info of the currently selected building
    void SelectBuilding()
    {
        //Don't be holding a placeable building or in a menu
        if (objectToPlace != null || !canSelectBuildings) return;

        Ray ray = mainCamera.ScreenPointToRay(controls.Camera.MousePosition.ReadValue<Vector2>());

        if (Physics.Raycast(ray, out RaycastHit hit, 100, buildingMask))
        {
            //Test to see if the building is valid
            hit.collider.gameObject.TryGetComponent(out BuildingMaster master);

            if (master != null) //Found a building
            {
                StartCoroutine(FadeInCanvasGroup(selectedBuildingUIGroup));
                selectedBuilding = master;
                UpdateSelectedBuildingInfo();
            }
        }
        else //No building found
        {
            selectedBuilding = null;
            StartCoroutine(FadeOutCanvasGroup(selectedBuildingUIGroup));
        }
    }

    //Update selected building info in the bottom right UI box
    //Need to add other buildings and info lines
    public void UpdateSelectedBuildingInfo()
    {
        if (!selectedBuilding) return;

        //if (selectedBuilding.displayName == "House")
        //{
        //    EnergyInfo.text = $"Population: +{selectedBuilding.populationImpact}";
        //    PopulationInfo.text = $"Food: {selectedBuilding.foodImpact}";
        //}
        selectedBuildingName.text = selectedBuilding.displayName;
    }

    //Registers the selected building to be deconstructed by the game manager
    void MarkForDeconstruction()
    {
        if (selectedBuilding)
        {
            selectedBuilding.TryGetComponent(out BuildingTree t);
            if (t) //If tree 
            {
                GameManager.Instance.RegisterForLogging(t);
            }
            else //If building
            {
                GameManager.Instance.RegisterForDeconstruction(selectedBuilding);
            }
        }
    }

    //Cache for less garbage collection
    readonly WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

    //Used to fade the bottom right info box out when clicked off
    IEnumerator FadeOutCanvasGroup(CanvasGroup cg)
    {
        while (cg.alpha != 0)
        {
            cg.alpha = Mathf.MoveTowards(cg.alpha, 0, Time.deltaTime * canvasFadeSpeed);
            yield return waitForEndOfFrame;
        }
    }
    //Used to fade the bottom right info box in when click on
    IEnumerator FadeInCanvasGroup(CanvasGroup cg)
    {
        while (cg.alpha != 1)
        {
            cg.alpha = Mathf.MoveTowards(cg.alpha, 1, Time.deltaTime * canvasFadeSpeed);
            yield return waitForEndOfFrame;
        }
    }

    //Spawns the real building once the ghost version is placed
    public void SpawnBuilding(int num)
    {
        if (objectToPlace != null) Destroy(objectToPlace.gameObject);
        InitializeWithObject(buildingList[num]);
        tr.enabled = true;
        replaceObject = num;
    }

    //Rotate the direction the building is facing
    void Rotate()
    {
        if (objectToPlace == null || !objectToPlace.canRotate) return;
        objectToPlace.AddRotation((int)controls.BuildingPlacement.Rotate.ReadValue<float>());
    }

    //Places the building onto the grid and marks those cells as occupied
    void PlaceBuilding()
    {
        if (objectToPlace == null) return;
        objectToPlace.TryGetComponent(out BuildingMaster bm);
        if (!(bm != null && gm.credits >= bm.buildCreditCost && gm.currentStoredBuildingMaterials >= bm.buildMatCost))
        {
            //play deny sound
            print("Not enough resources");
            return;
        }
        //Deduct cost of building
        gm.credits -= bm.buildCreditCost;
        gm.currentStoredBuildingMaterials -= bm.buildMatCost;
        GameMenu.Instance.RefreshBuildMenu();

        if (CanBePlaced(objectToPlace))
        {
            objectToPlace.Place();
            objectToPlace.GetComponent<BuildingMaster>().Initialize();

            //Calculate size on grid and claim the land
            Vector3Int start = gridLayout.WorldToCell(objectToPlace.GetStartPosition());
            TakeArea(start, objectToPlace.Size);

            if (!objectToPlace.CompareTag("Road")) objectToPlace.gameObject.layer = 7;
            placementRot = objectToPlace.currentRotation;
            objectToPlace = null;
            SpawnBuilding(replaceObject);
        }
        else
        {
            //Play denied sound
        }

        //Update hud since resources were spent
        GameMenu.Instance.RefreshHUD();
    }

    //Stop the placement of a building (Right Click)
    void CancelBuildingPlacement()
    {
        if (objectToPlace != null) Destroy(objectToPlace.gameObject);
        tr.enabled = false;
    }

    //Get the position of the cursor in world space
    public Vector3 GetMouseWorldPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(controls.Camera.MousePosition.ReadValue<Vector2>());
        if (Physics.Raycast(ray, out RaycastHit hit, 100, floorMask))
        {
            return hit.point;
        }
        else //If fail give a location outside of the playable area to force a fail for SnapCoordinateToGrid()
        {
            return new Vector3(0, 0, 1000);
        }
    }

    //Convert worldspace position to nearest cell position
    public Vector3 SnapCoordinateToGrid(Vector3 position)
    {
        Vector3Int cellPos = gridLayout.WorldToCell(position);
        position = grid.GetCellCenterWorld(cellPos);
        return position;
    }

    //Initialize the placement of the ghost building
    void InitializeWithObject(GameObject prefab)
    {
        Vector3 position = SnapCoordinateToGrid(Vector3.zero);

        GameObject obj = Instantiate(prefab, position, Quaternion.identity);
        objectToPlace = obj.GetComponent<PlaceableObject>();
        objectToPlace.importRot = placementRot;
        obj.AddComponent<ObjectDrag>();
    }

    //Gets the cells within a bounding box on the building grid
    static TileBase[] GetTilesBlock(BoundsInt area, Tilemap tilemap)
    {
        TileBase[] array = new TileBase[area.size.x * area.size.y * area.size.z];
        int counter = 0;
        foreach (Vector3Int v in area.allPositionsWithin)
        {
            Vector3Int pos = new Vector3Int(v.x, v.y, 0);
            array[counter] = tilemap.GetTile(pos);
            counter++;
        }
        return array;
    }

    //Is the object in a valid location
    //Is the valid location currently free/not occupied by other buildings
    bool CanBePlaced(PlaceableObject placeableObject)
    {
        //Create new bouding box to test again the building grid
        BoundsInt area = new BoundsInt
        {
            position = gridLayout.WorldToCell(objectToPlace.GetStartPosition()),
            size = placeableObject.Size
        };

        area.size = new Vector3Int(area.size.x + 1, area.size.y + 1, area.size.z);

        //Get all tiles within bounding box
        TileBase[] baseArray = GetTilesBlock(area, MainTilemap);
        foreach (TileBase b in baseArray)
        {
            if (b == buildingTile) //Is cell already occupied?
            {
                return false;
            }
        }
        //Return true only if all cells are free
        return true;
    }

    //Mark the specified cells using the position and size of the bounding box
    public void TakeArea(Vector3Int start, Vector3Int size)
    {
        MainTilemap.BoxFill(start, buildingTile, start.x, start.y,
                            start.x + size.x, start.y + size.y);
    }

    //Used for demolishing buildings
    //Frees up the tiles to be built on again
    public void ClearArea(Vector3Int start, Vector3Int size)
    {
        MainTilemap.BoxFill(start, null, start.x, start.y, start.x + size.x, start.y + size.y);
    }

}
