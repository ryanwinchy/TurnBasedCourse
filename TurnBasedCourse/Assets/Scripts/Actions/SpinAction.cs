using System;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{


    float totalSpinAmount;

    private void Update()
    {
        if (!isActive)
            return;


        float spinAddAmount = 360f * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, spinAddAmount, 0);

        totalSpinAmount += spinAddAmount;

        if (totalSpinAmount >= 360f)
        {
            ActionComplete();     //Base func, avoids repeated code on all actions. It is setting this action to inactive, and calling ClearBusy delegate, stored in onActionComplete var.
        }


    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)  //Spin. gridPosition redundant, but must override from base Take Action. Other actions need gridPos. You could actually pass in 'BaseParameters' class then have 'SpinParamters' child and cast to that, but long.
    {
        totalSpinAmount = 0;
        ActionStart(onActionComplete);                 //Base function, sets this action active, and stores ClearBusy delegate in onAction complete var.

    }


    public override string GetActionName() => "Spin";

    public override List<GridPosition> GetValidActionGridPositionList()       //For spin, valid position is the pos of the unit itself.
    {
        GridPosition unitGridPosition = unit.GetGridPosition();

        return new List<GridPosition> { unitGridPosition };   
    }

    public override int GetActionPointsCost()
    {
        return 2;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction 
        { 
            gridPosition = gridPosition,
            aiScore = 0,          //Will only do spin if cannot do anything else.
        };
    }
}
