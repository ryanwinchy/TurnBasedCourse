using System;
using System.Collections.Generic;
using UnityEngine;


//Script to manage the grid, like creating it.
//This script is the central hub for others to interact with. So will have a lot of publics and pass thru functions. Scripts should always access this, not 'GridSystem' which has the actual logic of the system.
public class LevelGrid : MonoBehaviour
{
    public static LevelGrid Instance { get; private set; }       //Singleton so easy for scripts to ref. Including prefabs which can't reference objects in scene, only in prefab.

    public const float FLOOR_HEIGHT = 3f;     //Height we have set between floors, same as wall game objects we set.

    public event EventHandler OnAnyUnitMovedGridPosition;

    List<GridSystem<GridObject>> gridSystemList;        //make grid system of grid objects, same as before. Made list so we can support multiple floors (multiple grid systems).

    [SerializeField] Transform gridDebugObjectPrefab;

    [SerializeField] public int width;
    [SerializeField] int height;
    [SerializeField] float cellSize;
    [SerializeField] int numFloors;     //How many floors game has. So how many grids.


    private void Awake()
    {
        if (Instance != null)          //Singleton. If duplicates, destroys the dupe. So one single instance.
        {
            Destroy(gameObject);
            return;
        }
        else
            Instance = this;

        gridSystemList = new List<GridSystem<GridObject>>();

        for (int floor = 0; floor < numFloors; floor++)
        {

            GridSystem<GridObject> gridSystem = new GridSystem<GridObject>(width, height, cellSize, floor, FLOOR_HEIGHT,
            (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition));    //grid system constructor. Setting up GridSystem of type GridObject (normal way we always did it).  Requires delegate function at end, we are passing in function to create grid objects. It says give a GridSystem and position and will return new grid object of that gridSystem and pos.
        
            gridSystemList.Add(gridSystem);
        }


        //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
    }

    private void Start()
    {
        Pathfinding.Instance.Setup(width, height, cellSize, numFloors);
    }

    GridSystem<GridObject> GetGridSystem(int floor)     //Gives the grid system for a specified floor, necessary now we have multiple floors.
    {
        return gridSystemList[floor];
    }

    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);               //On grid object assign the unit to it.
        gridObject.AddUnit(unit);
    }

    public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        return gridObject.GetUnitList();

        //Debug.Log("No unit found at " + gridPosition.ToString());
    }

    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);               //On grid object clear the unit assigned.
        gridObject.RemoveUnit(unit);
    }

    public void UnitMoveGridPosition(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition)
    {
        RemoveUnitAtGridPosition(fromGridPosition, unit);
        AddUnitAtGridPosition(toGridPosition, unit);

        OnAnyUnitMovedGridPosition?.Invoke(this, EventArgs.Empty);
    }                                                                             //made helper function 'GetFloor()' as here only had world pos.
    public GridPosition GetGridPosition(Vector3 worldPosition) => GetGridSystem(GetFloor(worldPosition)).GetGridPosition(worldPosition);  //This is a pass thru function so dont have to expose (make public) vars in this script or the entire grid system. Better abstraction.

    public int GetFloor(Vector3 worldPosition)
    {
        return Mathf.RoundToInt(worldPosition.y / FLOOR_HEIGHT);    //Floor 0: y of 0 / 3 height = floor 0. Floor 1: y pos of 3 / 3 height = floor 1. etc...
    }
    public Vector3 GetWorldPosition(GridPosition gridPosition) => GetGridSystem(gridPosition.floor).GetWorldPosition(gridPosition);  //This is a pass thru function so dont have to expose (make public) vars in this script or the entire grid system. Better abstraction.
    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        if (gridPosition.floor < 0 || gridPosition.floor >= numFloors)
            return false;
        else
            return GetGridSystem(gridPosition.floor).IsValidGridPosition(gridPosition);   //Pass thru so access LevelGrid not the actual grid system.
    }


    public int GetWidth() => GetGridSystem(0).GetWidth();      //Pass thrus. Assuming we have at least one grid system.
    public int GetHeight() => GetGridSystem(0).GetHeight();

    public int GetFloorAmount() => numFloors;
    public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        return gridObject.HasAnyUnit();            // True if has at least one unit.
    }

    public Unit GetUnitAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        return gridObject.GetUnit();
    }

    public IInteractable GetInteractableAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);

        return gridObject.GetInteractable();
    }

    public void SetInteractableAtGridPosition(IInteractable interactable, GridPosition gridPosition)
    {

        GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).SetInteractable(interactable);

    }


}
