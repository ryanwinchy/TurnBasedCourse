using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BaseAction : MonoBehaviour
{
    protected Unit unit;
    protected bool isActive;

    protected Action onActionComplete;            //Initialize delegate it receives and store in var.

    protected virtual void Awake()
    {
        unit = GetComponent<Unit>(); 
    }

    public abstract string GetActionName();       //abstract means children HAVE to implement.

    public abstract void TakeAction(GridPosition gridPosition, Action OnActionComplete);   //abstract, so template, children HAVE to implement. They provide code for what action does. All have OnActionComplete delegate which sets busy to false at end.

    public virtual bool IsValidActionGridPosition(GridPosition gridPosition)   //As this is common for all the child actions. Virtual just in case some actions that require diff logic.
    {
        List<GridPosition> validGridPositionList = GetValidActionGridPositionList();

        return validGridPositionList.Contains(gridPosition);  //If given pos is in list of valid positions, returns true.
    }

    public abstract List<GridPosition> GetValidActionGridPositionList();

}
