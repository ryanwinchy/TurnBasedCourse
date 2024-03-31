using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    [SerializeField] Animator unitAnimator;
    Vector3 targetPosition;

    [SerializeField] int maxMoveDistance = 4;

    protected override void Awake()
    {
        base.Awake();      //Runs parent awake, which gets unit ref.

        targetPosition = transform.position;  //By default, unit does not move.
    }

    private void Update()
    {
        if (!isActive)          //Dont move if move action is not active, or two things trying to transform game object at once.
            return;

        float stoppingDistance = 0.1f;

        Vector3 moveDirection = (targetPosition - transform.position).normalized;     //Gives us direction and magnitude (distance between two points), we only need direction so can use normalize.

        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)   //When have a gap bigger than stopping distance (this prevents twitching when gets there)
        {

            float moveSpeed = 4f;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;     //Makes framerate independent. Otherwise will move faster if higher frame rate cos update runs more (it runs once per frame).

            unitAnimator.SetBool("Moving", true);
        }
        else
        {
            isActive = false;
            unitAnimator.SetBool("Moving", false);
            onActionComplete();   //Running onActionComplete delegate, which is a ref to the ClearBusy() function to set 'isBusy' in unit action system to false.
        }

        float rotateSpeed = 10f;  //So rotates faster.
        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);      //Make char face where he is going.
                                                                                                               //Lerp is applying smoothing between current transform.forward and moveDirection, the end result. Will move towards it each frame smoothly. Will move most at start, as transform is updating , so gap smaller and smaller, looks nice. For linear, would just store original transform.
    }



    public override void TakeAction(GridPosition targetGridPosition, Action onActionComplete)        //Move unit to target pos.
    {
        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);       //For movement needs world pos, so takes in grid pos and converts it for movement.
        isActive = true;

        this.onActionComplete = onActionComplete;
    }


    public override List<GridPosition> GetValidActionGridPositionList()       //Gets list of grid objects can move to.
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)    //Left to right, cycle thru valid grid positions. Eg if dist 1: -1, 0 and 1.
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)    //Same on z.
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);       //Cycled vals are an offset. Like -1, 0 and 1 if move dist of 1.
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;       //Take current pos and cycle thru all offsets. So if at 4,4 , can go 4,3, 4,4 , 4,5 etc...

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))      //if invalid (is negative or outside grid bounds), moves on to next.
                    continue;

                if (testGridPosition == unitGridPosition)       //Pos unit is already at, ignore it.
                    continue;

                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))        //if pos alreayd occupied by a unit, dont include.
                    continue;

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override string GetActionName() => "Move";
}


