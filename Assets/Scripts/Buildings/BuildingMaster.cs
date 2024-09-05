using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//Building parent that contains all stats a building needs
public class BuildingMaster : MonoBehaviour
{
    public string displayName;
    public TMP_Text counterText;
    public int
        foodImpact,
        energyImpact,
        waterImpact,
        climateImpact,
        happinessImpact,
        populationImpact,
        buildingMatsImpact,
        logsImpact,

        buildTime,
        deconstructTime,
        deconstructionCurrent,
        buildMatCost,
        buildMatRefund,
        buildCreditCost,

        /*Logs gained on deconstruction*/
        logsRefund,

        followZoomAmount;

    public bool
        hasConnectedRoad,
        canBeDeconstructed,
        isDeconstructing,
        isBuilding,
        isDisabled;

    BoxCollider roadChecker;
    public LayerMask roadMask = 64;
    public LayerMask buildingMask = 128;
    public LayerMask roadCheckerMask = 1024;
    public SpriteRenderer deconstructSprite;


    //Needs to be overriden by children
    //DO NOT DELETE
    public virtual void Initialize() { }

    
    [ContextMenu("Check for Road")]
    public void CheckForRoadConnection()
    {
        if (roadChecker == null) return;

        hasConnectedRoad = Physics.OverlapBox(roadChecker.transform.position, roadChecker.size * .5f, Quaternion.identity, roadMask).Length > 0;
    }
#if UNITY_EDITOR
    [ContextMenu("Register for Deconstruction")]
    void DebugDeconstruction()
    {
        GameManager.Instance.RegisterForDeconstruction(this);
    }
#endif
}
