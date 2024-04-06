using System;
using System.Collections.Generic;
using UnityEngine;

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
        this.onActionComplete = onActionComplete;     //Storing delegate function received in the var. In this case, storing 'clear busy' in action system script. Pass in whatever func you want to do on action completed.

        OnAnyActionStarted?.Invoke(this, EventArgs.Empty);   //Fires event. You want event fire after everything else, so make sure children call 'ActionStart' last in their setup.
    }

    protected void ActionComplete()
    {
        isActive = false;
        onActionComplete();     //Calls stored delegate function, clear busy in action system script. 'SetTakingTurn' for enemy AI.

        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);       //Fires event. You want event fire after everything else, so make sure children call 'ActionStart' last in their setup.
    }

    public Unit GetUnit() => unit;




    public EnemyAIAction GetBestEnemyAIAction()            //This will be performed on the child action in question. All have this as its on base.
    {
        List<EnemyAIAction> enemyAIActionList = new List<EnemyAIAction>();

        List<GridPosition> validGridPositionList = GetValidActionGridPositionList();        //This will be performed on the child action in question, calling its own version of GetValidActionGridPositionList(). Eg for shooting, returns list of all grid positions in valid shoot range.

        foreach (GridPosition gridPosition in validGridPositionList)   //Cycle thru all valid grid positions for this action (this is on base so will run on all children actions).
        {
            EnemyAIAction enemyAIAction = GetEnemyAIAction(gridPosition);    //Generate enemy ai action (score) for all valid grid positions for this action, like moving range or shooting range for eg.
            enemyAIActionList.Add(enemyAIAction);             //Make list of eligible actions.
        }

        if (enemyAIActionList.Count > 0)         //Sort valid ai action scores to get best.
        {
            enemyAIActionList.Sort((EnemyAIAction a, EnemyAIAction b) => b.aiScore - a.aiScore);      //This built in function will sort list by aiValue.

            return enemyAIActionList[0];       //Sorted, so best one will be first now.
        }
        else      //No actions.
            return null;
    }

    public abstract EnemyAIAction GetEnemyAIAction(GridPosition gridPosition);      //Every action script has to implement. Gets action on a given grid pos, and calcs aiScore so can sort to best.




}
