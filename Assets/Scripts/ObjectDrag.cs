using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used on the placeable ghosts version of buildings
public class ObjectDrag : MonoBehaviour
{
    Vector3 offset;

    private void LateUpdate()
    {
        //When placing a building, move the ghost version to the cursor
        //Then snap to the closest grid cells
        Vector3 pos = BuildingSystem.Instance.GetMouseWorldPosition() + offset;
        transform.position = BuildingSystem.Instance.SnapCoordinateToGrid(pos);
    }
}
