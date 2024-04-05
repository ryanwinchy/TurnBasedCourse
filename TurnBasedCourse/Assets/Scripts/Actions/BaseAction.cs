using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BaseAction : MonoBehaviour
{
    public static event EventHandler OnAnyActionStarted;   //static event is same as normal, but belongs to class itself, not an instance.  So runs on ANY action started.
    public static event EventHandler OnAnyActionCompleted;   //static event is same as normal, but belongs to class itself, not an instance.  So runs on ANY action started.

    protected Unit unit;
    protected bool isActive;

    protected Action onActionComplete;            //Initialize delegate it receives and store in var.

    protected virtual void Awake()
    {
        unit = GetComponent<Unit>(); 
    }

    public abstract string GetActionName();       //abstract means children HAVE to implement.

    public abstract void TakeAction(GridPosition gridPosition, Action onActionComplete);   //abstract, so template, children HAVE to implement. They provide code for what action does. All have OnActionComplete delegate which sets busy to false at end.

    public virtual bool IsValidActionGridPosition(GridPosition gridPosition)   //As this is common for all the child actions. Virtual just in case some actions that require diff logic.
    {
        List<GridPosition> validGridPositionList = GetValidActionGridPositionList();

        return validGridPositionList.Contains(gridPosition);  //If given pos is in list of valid positions, returns true.
    }

    public abstract List<GridPosition> GetValidActionGridPositionList();

    public virtual int GetActionPointsCost()  //Basically a default, children actions can replace if they choose. Perhaps attack is 2 points for eg.
    {
        return 1;
    }

    protected void ActionStart(Action onActionComplete)        //All children had this repeated code so put on base.
    {
        isActive = true;                                    //sets the action to active.
        this.onActionComplete = onActionComplete;     //Storing delegate function received in the var. In this case, storing 'clear busy' in action system script.

        OnAnyActionStarted?.Invoke(this, EventArgs.Empty);   //Fires event. You want event fire after everything else, so make sure children call 'ActionStart' last in their setup.
    }

    protected void ActionComplete()
    {
        isActive = false;
        onActionComplete();     //Calls stored delegate function, clear busy in action system script.
        
        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);       //Fires event. You want event fire after everything else, so make sure children call 'ActionStart' last in their setup.
    }



    public Unit GetUnit() => unit;
}
