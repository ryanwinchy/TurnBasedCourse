using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingLinkMonobehavior : MonoBehaviour
{
    public Vector3 linkPositionA;
    public Vector3 linkPositionB;

    public PathfindingLink GetPathfindingLink()         //Converts these to grid positions and returns as pathFindingLinks.
    {
        return new PathfindingLink { gridPositionA = LevelGrid.Instance.GetGridPosition(linkPositionA), gridPositionB = LevelGrid.Instance.GetGridPosition(linkPositionB) };
    }      


}
