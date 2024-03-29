using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Instantiated in every grid space.
//No monobehaviour so can make constructor.
public class GridObject
{
    GridSystem gridSystem;
    public GridPosition gridPosition;

    public GridObject(GridSystem gridSystem, GridPosition gridPosition)
    {
        this.gridSystem = gridSystem;
        this.gridPosition = gridPosition;
    }

    public override string ToString()       //All objects have to string. This by default returns the struct name, 'GridPosition'. We want it to return the vals.
    {
        return gridPosition.ToString();
    }
}
