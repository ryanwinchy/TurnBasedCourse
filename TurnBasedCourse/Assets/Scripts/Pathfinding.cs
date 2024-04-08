using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TerrainTools;

public class Pathfinding : MonoBehaviour
{

    public static Pathfinding Instance { get; private set; }          //Singleton. Any class can read, only this class can set. Properties are Pascal case.


    const int MOVE_STRAIGHT_COST = 10;     //See notes to see why.
    const int MOVE_DIAGONAL_COST = 14;

    int width;
    int height;
    float cellSize;

    [SerializeField] Transform gridDebugObjectPrefab;

    [SerializeField] LayerMask obstaclesLayerMask;

    GridSystem<PathNode> gridSystem;


    private void Awake()
    {

        if (Instance != null)          //Singleton. If duplicates, destroys the dupe. So one single instance.
        {
            Destroy(gameObject);
            return;
        }
        else
            Instance = this;


    }

    public void Setup(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridSystem = new GridSystem<PathNode>(width, height, cellSize,                                          //Creates grid system of PathNodes (instead of GridObjects, same thing but different grid, for a* algo).
    (GridSystem<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition));     //Passes in delegate function, give a gridsystem of pathNodes and a position, function will create path node at position (Squares in grid). This function goes into GridSystem thru a delegate called 'create grid object'.

        //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);


        //OBSTACLE SETUP.
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)           //Cycle thru whole grid.
            {
                GridPosition gridPosition = new GridPosition(x, z);    

                Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);   //Raycast origin, world pos of grid square.
                float raycastOffsetDistance = 5f;       // This offset is so the origin of the raycast is below the grid square. if it is on it, and wall is on y = 0, the raycast wont work.
                if (Physics.Raycast(worldPosition + Vector3.down * raycastOffsetDistance, Vector3.up, raycastOffsetDistance * 2, obstaclesLayerMask))   //Fire raycast from -5 below grid pos, shoot up 10, look for obstacles layer.
                {
                    GetNode(x, z).SetIsWalkable(false);    //Raycast returns true if hit something.
                }
            
            
            
            }
        }
    }


    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength)   //params for point A and B, SEE MY NOTES. Returns entire path , list of grid positions.
    {
        List<PathNode> openList = new List<PathNode>();           //Nodes left to search
        List<PathNode> closedList = new List<PathNode>();      //Nodes already searched.

        PathNode startNode = gridSystem.GetGridObject(startGridPosition);  //Converts grid pos to grid object (pathNode as this is pathfinding grid instead of normal grid).
        PathNode endNode = gridSystem.GetGridObject(endGridPosition);  //Converts grid pos to grid object (pathNode as this is pathfinding grid instead of normal grid).


        openList.Add(startNode);     //Add start node to ones left to search. 

        for (int x = 0; x < gridSystem.GetWidth(); x++)
        {
            for (int z = 0; z < gridSystem.GetHeight(); z++)     //Cycle thru whole PathNode Gridsystem.
            {
                GridPosition gridPosition = new GridPosition(x, z);
                PathNode pathNode = gridSystem.GetGridObject(gridPosition);     //Converts to grid Pos to pathNode (same as grid object but for path finding grid).

                pathNode.SetGCost(int.MaxValue);
                pathNode.SetHCost(0);
                pathNode.CalculateFCost();
                pathNode.ResetCameFromPathNode();
            }
        }

        startNode.SetGCost(0);                                                          //Initialize start node. Movement cost is 0 as its start node.
        startNode.SetHCost(CalculateDistance(startGridPosition, endGridPosition));          //Calc distance between start and end nodes for H value on start node.
        startNode.CalculateFCost();

        while (openList.Count > 0)       //While still have nodes to search.
        {
            PathNode currentNode = GetLowestFCostPathNode(openList);

            if (currentNode == endNode)      //Reached final node.
            {
                pathLength = endNode.GetFCost();
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);    //Indicates have searched this node.
            closedList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode)) //Check all neighbours of current node.
            {
                if (closedList.Contains(neighbourNode))  //Already searched this one, continue.
                    continue;

                if (!neighbourNode.IsWalkable())      //If neighbour isnt walkable, add to closed list, skip this node.
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.GetGCost() + CalculateDistance(currentNode.GetGridPosition(), neighbourNode.GetGridPosition());   //G cost of start node to this neighbour node.

                if (tentativeGCost < neighbourNode.GetGCost())    //Have found better path to get to neighbour node from current node. Like we were doing two straights, now can be one diagonal.
                {
                    neighbourNode.SetCameFromPathNode(currentNode);
                    neighbourNode.SetGCost(tentativeGCost);
                    neighbourNode.SetHCost(CalculateDistance(neighbourNode.GetGridPosition(), endGridPosition));
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode)) //If neighbour node not in open list, add it in so we can search it.
                    {
                        openList.Add(neighbourNode);
                    }

                }
            }
        }

        //NO MORE NODES TO CHECK, NO PATH FOUND.
        pathLength = 0;
        return null;
    }


    public int CalculateDistance(GridPosition gridPositionA, GridPosition gridPositionB)     //Calcs straight distance, no walls or anything. This is for heuristic in a* algo.
    {
        GridPosition gridPositionDistance = gridPositionA - gridPositionB;

        int distance = Mathf.Abs(gridPositionDistance.x) + Mathf.Abs(gridPositionDistance.z);  //abs takes only positive values.

        int xDistance = Mathf.Abs(gridPositionDistance.x); //So can work out striaghts and diagonals (see my notes on a* pathfinding).
        int zDistance = Mathf.Abs(gridPositionDistance.z);
        int remaining = Mathf.Abs(xDistance - zDistance); 

        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) + MOVE_STRAIGHT_COST * remaining;     //yes makes sense, visualize with grid I drew in notes.

    }

    PathNode GetLowestFCostPathNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostPathNode = pathNodeList[0];

        for (int i = 0; i < pathNodeList.Count; i++)     //Cycle thru list.
        {
            if (pathNodeList[i].GetFCost() < lowestFCostPathNode.GetFCost())     //get lowest F cost from list.
                lowestFCostPathNode = pathNodeList[i];
        }

        return lowestFCostPathNode;

    }

    PathNode GetNode(int x, int z)
    {
        return gridSystem.GetGridObject(new GridPosition(x, z));   //Get grid object (in this case it is pathNode, which is a grid object on the pathfinding grid), using a new grid position of given (x,y), as var doesnt exist for that grid position.
    }

    List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode > neighbourList = new List<PathNode>();

        GridPosition gridPosition = currentNode.GetGridPosition();

        if (gridPosition.x - 1 >= 0)  //Conditionals here are just validating that it is on grid.
        {
            //Left
            neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z));
            if (gridPosition.z - 1 >= 0)
            {
                //Left Down
                neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1));
            }
            if (gridPosition.z + 1 < gridSystem.GetHeight())
            {
                //Left Up
                neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1));
            }
  
        }

        if (gridPosition.x + 1 < gridSystem.GetHeight())
        {
            //Right
            neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z));
            if (gridPosition.z - 1 >= 0)
            {
                //Right Down
                neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1));
            }
            if (gridPosition.z + 1 < gridSystem.GetHeight())
            {
                //Right Up
                neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1));
            }
        }

        if (gridPosition.z - 1 >= 0)
        {
            //Below
            neighbourList.Add(GetNode(gridPosition.x, gridPosition.z - 1));
        }
        if (gridPosition.z + 1 < gridSystem.GetHeight())
        {
            //Above
            neighbourList.Add(GetNode(gridPosition.x, gridPosition.z + 1));
        }


        return neighbourList;
    }

    List<GridPosition> CalculatePath(PathNode endNode)
    {
        List<PathNode> pathNodeList = new List<PathNode>();

        pathNodeList.Add(endNode);

        PathNode currentNode = endNode;

        while (currentNode.GetCameFromPathNode() != null)   //While has a prev node connected to it.
        {
            pathNodeList.Add(currentNode.GetCameFromPathNode());       //Add linked node.
            currentNode = currentNode.GetCameFromPathNode();      //Cycle thru next linked node. Will eventually have the path of nodes.
        }

        pathNodeList.Reverse();       //So list in right order.

        List<GridPosition> gridPositionList = new List<GridPosition>();

        foreach (PathNode pathNode in pathNodeList)
        {
            gridPositionList.Add(pathNode.GetGridPosition());
        }

        return gridPositionList;

    }

    public bool IsWalkableGridPosition(GridPosition gridPosition) => gridSystem.GetGridObject(gridPosition).IsWalkable();

    public void SetIsWalkableGridPosition(GridPosition gridPosition, bool isWalkable) => gridSystem.GetGridObject(gridPosition).SetIsWalkable(isWalkable);


    //This tests if there is actually a path to get there, that its not unreachable.
    public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition) => FindPath(startGridPosition, endGridPosition, out int pathLength) != null;

    public int GetPathLength(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        FindPath(startGridPosition, endGridPosition, out int pathLength);

        return pathLength;
    }

}
