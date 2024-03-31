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
            onActionComplete();   //Running onActionComplete delegate, which is a ref to the ClearBusy() function to set 'isBusy' in unit action system to false.
            isActive = false;
        }


    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)  //Spin. gridPosition redundant, but must override from base Take Action. Other actions need gridPos. You could actually pass in 'BaseParameters' class then have 'SpinParamters' child and cast to that, but long.
    {
        isActive = true;
        totalSpinAmount = 0;

        this.onActionComplete = onActionComplete;       //Storing delegate (var storing function), func its storing is just to clear busy status in UnitActionSystem.
    }


    public override string GetActionName() => "Spin";

    public override List<GridPosition> GetValidActionGridPositionList()       //For spin, valid position is the pos of the unit itself.
    {
        GridPosition unitGridPosition = unit.GetGridPosition();

        return new List<GridPosition> { unitGridPosition };   
    }




}
