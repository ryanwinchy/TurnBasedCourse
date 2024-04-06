using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is each square on the grid, for use in the pathfinding algo for units to find optimum paths.
public class PathNode
{
    GridPosition gridPosition;      //position of node (square).

    int gCost;    //Components of a* algorithm, see my notes.
    int hCost;
    int fCost;
    PathNode cameFromPathNode;    //Stores previous path node it came from. So when get to end can loop back thru optimal path.
    public PathNode(GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;
    }

    public override string ToString()       //All objects have to string. This by default returns the struct name, 'GridPosition'. We want it to return the vals.
    {

        return gridPosition.ToString();
    }

    public int GetGCost() => gCost;
    public int GetHCost() => hCost;
    public int GetFCost() => fCost;

    public void SetGCost(int gCost) => this.gCost = gCost;
    public void SetHCost(int hCost) => this.hCost = hCost;

    public void CalculateFCost() => fCost = gCost + hCost;

    public void ResetCameFromPathNode() => cameFromPathNode = null;

    public void SetCameFromPathNode(PathNode pathNode) => cameFromPathNode = pathNode;

    public PathNode GetCameFromPathNode() =>  cameFromPathNode;

    public GridPosition GetGridPosition() => gridPosition;

}
