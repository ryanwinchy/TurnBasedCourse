using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractAction : BaseAction
{

    int maxInteractDistance = 1;

    private void Update()
    {
        if (!isActive)
            return;
    }
    public override string GetActionName()
    {
        return "Interact";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction { gridPosition = gridPosition, aiScore = 0 };       //Will only do if cant do anything else.
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxInteractDistance; x <= maxInteractDistance; x++)    //Left to right, cycle thru valid grid positions. Eg if dist 1: -1, 0 and 1.
        {
            for (int z = -maxInteractDistance; z <= maxInteractDistance; z++)    //Same on z.
            {
                GridPosition offsetGridPosition = new GridPosition(x, z, 0);       //Cycled vals are an offset. Like -1, 0 and 1 if move dist of 1.  0 on floor means do not offset floor, use same one.
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;       //Take current pos and cycle thru all offsets. So if at 4,4 , can go 4,3, 4,4 , 4,5 etc...

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))      //if invalid (is negative or outside grid bounds), moves on to next.
                    continue;

                IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(testGridPosition);    //Checks for door at that grid pos. Door script sets door at its gridPos.

                if (interactable == null)        //No door on this grid position.
                    continue;


                validGridPositionList.Add(testGridPosition);        //If get thru all checks, grid pos is valid.
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(gridPosition);    //Checks for door at that grid pos. Door script sets door at its gridPos.

        interactable.Interact(OnInteractComplete);

        ActionStart(onActionComplete);
    }

    void OnInteractComplete()
    {
        ActionComplete();
    }


}
