using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//When placing a road, detects the roads around it to see how it should fit to properly connect
//Really not efficient but it works
public class Road : BuildingMaster
{
    //Mesh shapes (T=T intersection, L = Right angle turn etc.)
    [SerializeField] Mesh TMesh, LMesh, straightMesh, XMesh, EndMesh, BoxedMesh;
    [SerializeField] MeshFilter mf;
    PlaceableObject po;

    public override void Initialize()
    {
        po = GetComponent<PlaceableObject>();
        UpdateState(true);
        BuildingSystem.Instance.UpdateSelectedBuildingInfo();

    }

    //Determines the appropriate mesh and rotation to use to match up with surrounding road pieces
    public void UpdateState(bool send)
    {
        //Gather all the surrounding buildings and check if they are roads
        //Really should have just had the buildings write to the grid a unique value to easily determine the type of building that occupies each cell
        transform.eulerAngles = Vector3.zero;
        Collider[] hitsUp = Physics.OverlapBox(transform.position + Vector3.forward, new Vector3(.25f, .25f, .25f), Quaternion.identity, roadMask);
        Collider[] hitsRight = Physics.OverlapBox(transform.position + Vector3.right, new Vector3(.25f, .25f, .25f), Quaternion.identity, roadMask);
        Collider[] hitsDown = Physics.OverlapBox(transform.position + Vector3.back, new Vector3(.25f, .25f, .25f), Quaternion.identity, roadMask);
        Collider[] hitsLeft = Physics.OverlapBox(transform.position + Vector3.left, new Vector3(.25f, .25f, .25f), Quaternion.identity, roadMask);

        if (send && Physics.OverlapBox(transform.position, new Vector3(1, 1, 1), Quaternion.identity, roadCheckerMask).Length > 0)
        {
            Collider[] houseUp = Physics.OverlapBox(transform.position + Vector3.forward, new Vector3(.25f, .25f, .25f), Quaternion.identity, buildingMask);
            Collider[] houseRight = Physics.OverlapBox(transform.position + Vector3.right, new Vector3(.25f, .25f, .25f), Quaternion.identity, buildingMask);
            Collider[] houseDown = Physics.OverlapBox(transform.position + Vector3.back, new Vector3(.25f, .25f, .25f), Quaternion.identity, buildingMask);
            Collider[] houseLeft = Physics.OverlapBox(transform.position + Vector3.left, new Vector3(.25f, .25f, .25f), Quaternion.identity, buildingMask);

            for (int i = 0; i < houseUp.Length; i++)
            {
                houseUp[i].GetComponent<BuildingMaster>().CheckForRoadConnection();
            }
            for (int i = 0; i < houseRight.Length; i++)
            {
                houseRight[i].GetComponent<BuildingMaster>().CheckForRoadConnection();
            }
            for (int i = 0; i < houseDown.Length; i++)
            {
                houseDown[i].GetComponent<BuildingMaster>().CheckForRoadConnection();
            }
            for (int i = 0; i < houseLeft.Length; i++)
            {
                houseLeft[i].GetComponent<BuildingMaster>().CheckForRoadConnection();
            }
        }


        int neighbourCount = 0;
        int neighbourScore = 0;
        //up=1; right=3; down=5; left=10;
        //Gives a unique value to every possibility
        //Not sure why I'm comparing tag to "Road" if already masking the overlapBox
        for (int i = 0; i < hitsUp.Length; i++)
        {
            if (hitsUp[i].CompareTag("Road"))
            {
                if (send) hitsUp[i].GetComponent<Road>().UpdateState(false);
                neighbourCount++;
                neighbourScore++;
                break;
            }
        }

        for (int i = 0; i < hitsRight.Length; i++)
        {
            if (hitsRight[i].CompareTag("Road"))
            {
                if (send) hitsRight[i].GetComponent<Road>().UpdateState(false);
                neighbourCount++;
                neighbourScore += 3;
                break;
            }
        }

        for (int i = 0; i < hitsDown.Length; i++)
        {
            if (hitsDown[i].CompareTag("Road"))
            {
                if (send) hitsDown[i].GetComponent<Road>().UpdateState(false);
                neighbourCount++;
                neighbourScore += 5;
                break;
            }
        }

        for (int i = 0; i < hitsLeft.Length; i++)
        {
            if (hitsLeft[i].CompareTag("Road"))
            {
                if (send) hitsLeft[i].GetComponent<Road>().UpdateState(false);
                neighbourCount++;
                neighbourScore += 10;
                break;
            }
        }

        //All below determine mesh and orientation based on gathered results
        if (neighbourCount == 0)
        {
            mf.mesh = BoxedMesh;
            return;
        }

        if (neighbourCount == 4)
        {
            mf.mesh = XMesh;
            return;
        }

        if (neighbourCount == 3)
        {
            mf.mesh = TMesh;
            if (neighbourScore == 18)
            {
                po.AddRotation(1);
                return;
            }
            if (neighbourScore == 16)
            {
                po.AddRotation(2);
                return;
            }
            if (neighbourScore == 14)
            {
                po.AddRotation(-1);
                return;
            }
            return;
        }

        if (neighbourCount == 2)
        {
            mf.mesh = straightMesh;
            if (neighbourScore == 6) return;

            if (neighbourScore == 13)
            {
                po.AddRotation(1);
                return;
            }

            mf.mesh = LMesh;

            if (neighbourScore == 4) return;

            if (neighbourScore == 8)
            {
                po.AddRotation(1);
                return;
            }
            if (neighbourScore == 15)
            {
                po.AddRotation(2);
                return;
            }
            if (neighbourScore == 11)
            {
                po.AddRotation(-1);
                return;
            }
        }

        if (neighbourCount == 1)
        {
            mf.mesh = EndMesh;
            if (neighbourScore == 5) return;

            if (neighbourScore == 10)
            {
                po.AddRotation(1);
                return;
            }
            if (neighbourScore == 1)
            {
                po.AddRotation(2);
                return;
            }
            po.AddRotation(-1);
        }

    }
}
