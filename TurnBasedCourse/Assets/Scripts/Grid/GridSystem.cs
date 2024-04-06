using UnityEngine;
using System;

//Generic GridSystem so can reuse for path finding instead of duplicating grid.
public class GridSystem<TGridObject>
{
    int width;      //Of whole grid.
    int height;

    float cellSize;

    TGridObject[,] gridObjectsArray;     //2d array of gridObjects. 2d means has two dimensions, can reference objects with two indexs, like 1,1.

    public GridSystem(int width, int height, float cellSize, Func<GridSystem<TGridObject>, GridPosition, TGridObject> createGridObject)       //Func is delegate. Params are GridSystem, GridPosition, and returns TGridObject. names this delegate param createGridObject, then use the delegate to create grid objects (squares in grid).
    {                                                                                  //Func at end is because GridObject requires a ref to GridSystem and GridPosition , so we use a Func delegate which receives the GridSystem and GridPosition and constructs each invidual game object.
        this.width = width;                                          
        this.height = height;
        this.cellSize = cellSize;

        gridObjectsArray = new TGridObject[width, height];     //Initializes 2d array of size width (of width grid), and height (of total grid).

        for (int x = 0; x < this.width; x++)     //Cycle thru width and height of grid, creating tiles.
        {
            for (int z = 0; z < this.height; z++)   //Z because grid is X and Z. Height refers to concept of 2d grid.
            {
                GridPosition gridPosition = new GridPosition(x, z);
                gridObjectsArray[x, z] = createGridObject(this, gridPosition);         //Pass in grid position its iterating thru, and this GridSystem script to construct a Grid Object.
            }
        }

    }

    public Vector3 GetWorldPosition(GridPosition gridPosition) => new Vector3(gridPosition.x, 0, gridPosition.z) * cellSize;            //Converts width and height of grid pos given to world point.

    public GridPosition GetGridPosition(Vector3 _worldPosition)       //comverts world pos to grid pos, like one to right one up is (1,1). As if cell size is diff for each grid, gonna be different.
    {
        return new GridPosition(Mathf.RoundToInt(_worldPosition.x / cellSize), Mathf.RoundToInt(_worldPosition.z / cellSize));        //Pos has to be ints.
    }

    public void CreateDebugObjects(Transform debugPrefab)
    {
        for (int x = 0; x < this.width; x++)     //Cycle thru width and height of grid, creating tiles.
        {
            for (int z = 0; z < this.height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);

                Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);    //Have to do Gameobject.Instantiate, as no monobehaviour.

                GridDebugObject gridDebugObect = debugTransform.GetComponent<GridDebugObject>();
                gridDebugObect.SetGridObject(gridObjectsArray[x, z]);    //paste in the grid objects (each tile) to the debug tile, so can overlay text on each one.
            }
        }
    }

    public TGridObject GetGridObject(GridPosition gridPosition)
    {
        return gridObjectsArray[gridPosition.x, gridPosition.z];
    }

    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        return gridPosition.x >= 0 &&       //Returns true if within bounds of grid.
                gridPosition.z >= 0 &&
                gridPosition.x < width &&
                gridPosition.z < height;
    }

    public int GetWidth() => width;
    public int GetHeight() => height;

}
