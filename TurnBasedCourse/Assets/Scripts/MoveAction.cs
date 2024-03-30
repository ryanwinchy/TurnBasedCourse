using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoveAction : MonoBehaviour
{
    [SerializeField] Animator unitAnimator;
    Vector3 targetPosition;

    Unit unit;

    [SerializeField] int maxMoveDistance = 4;

    private void Awake()
    {
        unit = GetComponent<Unit>();
        targetPosition = transform.position;  //By default, unit does not move.
    }

    private void Update()
    {
        float stoppingDistance = 0.1f;

        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)   //When have a gap bigger than stopping distance (this prevents twitching when gets there)
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;     //Gives us direction and magnitude (distance between two points), we only need direction so can use normalize.

            float moveSpeed = 4f;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;     //Makes framerate independent. Otherwise will move faster if higher frame rate cos update runs more (it runs once per frame).

            float rotateSpeed = 10f;  //So rotates faster.
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);      //Make char face where he is going.
                                                                                                                   //Lerp is applying smoothing between current transform.forward and moveDirection, the end result. Will move towards it each frame smoothly. Will move most at start, as transform is updating , so gap smaller and smaller, looks nice. For linear, would just store original transform.
            unitAnimator.SetBool("Moving", true);
        }
        else
            unitAnimator.SetBool("Moving", false);
    }



    public void MoveUnitToTarget(GridPosition targetGridPosition)
    {
        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);       //For movement needs world pos, so takes in grid pos and converts it for movement.
    }

    public bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        List<GridPosition> validGridPositionList = GetValidActionGridPositionList();

        return validGridPositionList.Contains(gridPosition);  //If given pos is in list of valid positions, returns true.
    }

    public List<GridPosition> GetValidActionGridPositionList()       //Gets list of grid objects can move to.
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

}


