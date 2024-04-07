using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;


//Script to manage the grid, like creating it.
//This script is the central hub for others to interact with. So will have a lot of publics and pass thru functions. Scripts should always access this, not 'GridSystem' which has the actual logic of the system.
public class LevelGrid : MonoBehaviour
{
    public static LevelGrid Instance { get; private set; }       //Singleton so easy for scripts to ref. Including prefabs which can't reference objects in scene, only in prefab.

    public event EventHandler OnAnyUnitMovedGridPosition;

    GridSystem<GridObject> gridSystem;        //make grid system of grid objects, same as before.

    [SerializeField] Transform gridDebugObjectPrefab;

    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] float cellSize;


    private void Awake()
    {
        if (Instance != null)          //Singleton. If duplicates, destroys the dupe. So one single instance.
        {
            Destroy(gameObject);
            return;
        }
        else
            Instance = this;

        gridSystem = new GridSystem<GridObject>(width, height, cellSize, 
            (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition));    //grid system constructor. Setting up GridSystem of type GridObject (normal way we always did it).  Requires delegate function at end, we are passing in function to create grid objects. It says give a GridSystem and position and will return new grid object of that gridSystem and pos.

        //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
    }

    private void Start()
    {
        Pathfinding.Instance.Setup(width, height, cellSize);
    }

    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);               //On grid object assign the unit to it.
        gridObject.AddUnit(unit);
    }

    public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetUnitList();

        //Debug.Log("No unit found at " + gridPosition.ToString());
    }

    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);               //On grid object clear the unit assigned.
        gridObject.RemoveUnit(unit);
    }

    public void UnitMoveGridPosition(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition)
    {
        RemoveUnitAtGridPosition(fromGridPosition, unit);
        AddUnitAtGridPosition(toGridPosition, unit);

        OnAnyUnitMovedGridPosition?.Invoke(this, EventArgs.Empty);
    }
    public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);  //This is a pass thru function so dont have to expose (make public) vars in this script or the entire grid system. Better abstraction.

    public Vector3 GetWorldPosition(GridPosition gridPosition) => gridSystem.GetWorldPosition(gridPosition);  //This is a pass thru function so dont have to expose (make public) vars in this script or the entire grid system. Better abstraction.
    public bool IsValidGridPosition(GridPosition gridPosition) => gridSystem.IsValidGridPosition(gridPosition);   //Pass thru so access LevelGrid not the actual grid system.

    public int GetWidth() => gridSystem.GetWidth();      //Pass thrus.
    public int GetHeight() => gridSystem.GetHeight();   
    public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.HasAnyUnit();            // True if has at least one unit.
    }

    public Unit GetUnitAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetUnit();
    }


}
