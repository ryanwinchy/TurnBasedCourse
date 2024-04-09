using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Instantiated in every grid space.
//No monobehaviour so can make constructor.
public class GridObject
{
    GridSystem<GridObject> gridSystem;
    public GridPosition gridPosition;

    public List<Unit> unitList;    //Has to be list as when moving there is a chance for two units to be on one tile for a second.

    IInteractable interactable;

    public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridPosition)
    {
        this.gridSystem = gridSystem;
        this.gridPosition = gridPosition;

        unitList = new List<Unit>();
    }

    public override string ToString()       //All objects have to string. This by default returns the struct name, 'GridPosition'. We want it to return the vals.
    {
        string unitString = "";
        foreach (Unit unit in unitList)
        {
            unitString += unit + "\n";
        }
        return gridPosition.ToString() +  "\n" + unitString;
    }

    public void AddUnit(Unit unit)
    {
        unitList.Add(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        unitList.Remove(unit);
    }

    public List<Unit> GetUnitList()
    {
        return unitList;
    }

    public bool HasAnyUnit()
    {
        return unitList.Count > 0;
    }

    public Unit GetUnit()
    {
        if (HasAnyUnit())
            return unitList[0];     //Return first unit in list on this grid space. Called when idle, so can only ever be one. Only time two is when cross eachother during movement.
        else
            return null;
    }

    public IInteractable GetInteractable() => interactable;

    public void SetInteractable(IInteractable interactable) => this.interactable = interactable;

}
