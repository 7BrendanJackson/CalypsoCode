using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Designates an object can be placed onto the building grid
//Used by the ghost version of a building when placing it
public class PlaceableObject : MonoBehaviour
{
    public bool placed, canRotate = true;
    public Vector3Int Size;
    Vector3[] Vertices;
    public int currentRotation = 0;
    public int importRot = 0;

    //Breaks the bounding box collider down into vertex offsets
    void GetColliderVertexPositionLocal()
    {
        BoxCollider bc = GetComponent<BoxCollider>();
        Vertices = new Vector3[4];
        Vertices[0] = bc.center + new Vector3(-bc.size.x, -bc.size.y, -bc.size.z) * .5f;
        Vertices[1] = bc.center + new Vector3(bc.size.x, -bc.size.y, -bc.size.z) * .5f;
        Vertices[2] = bc.center + new Vector3(bc.size.x, -bc.size.y, bc.size.z) * .5f;
        Vertices[3] = bc.center + new Vector3(-bc.size.x, -bc.size.y, bc.size.z) * .5f;
    }

    //Calculate the size in grid cells based on vertices
    void CalculateSizeInCells()
    {
        Vector3Int[] vertices = new Vector3Int[Vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldPos = transform.TransformPoint(Vertices[i]);
            vertices[i] = BuildingSystem.Instance.gridLayout.WorldToCell(worldPos);
        }

        Size = new Vector3Int(Mathf.Abs((vertices[0] - vertices[1]).x), Mathf.Abs((vertices[0] - vertices[3]).y), 1);
    }

    public Vector3 GetStartPosition()
    {
        return transform.TransformPoint(Vertices[0]);
    }

    private void Start()
    {
        GetColliderVertexPositionLocal();
        CalculateSizeInCells();
    }

    //Finalises placing the object
    public virtual void Place()
    {
        ObjectDrag drag = GetComponent<ObjectDrag>();
        Destroy(drag);

        placed = true;
    }

    //Rotate the placeable building 90degrees
    public void AddRotation(int dir)
    {
        currentRotation = (currentRotation + 4 + dir) % 4;

        transform.Rotate(new Vector3(0, dir * 90, 0));

        Size = new Vector3Int(Size.y, Size.x, 1);

        Vector3[] vertices = new Vector3[Vertices.Length];

        //Resort vertices for new rotation
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = Vertices[(i + dir + Vertices.Length) % Vertices.Length];
        }

        Vertices = vertices;
    }
}
