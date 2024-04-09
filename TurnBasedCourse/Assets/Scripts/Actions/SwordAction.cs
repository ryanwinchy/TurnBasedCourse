using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class SwordAction : BaseAction
{

    public static event EventHandler OnAnySwordHit;

    public event EventHandler OnSwordActionStarted;   //One specific sword instance starting and completing. If was any would be whack.
    public event EventHandler OnSwordActionCompleted;

    int maxSwordDistance = 1;

    enum State { SwingingSwordBeforeHit, SwingSwordAfterHit }

    State state;
    float stateTimer;
    Unit targetUnit;

    private void Update()
    {
        if (!isActive) 
            return;

        stateTimer -= Time.deltaTime;   //So happens for all states.

        switch (state)
        {
            case State.SwingingSwordBeforeHit:
                AimAtTarget();
                break;
            case State.SwingSwordAfterHit:
                break;
        }

        if (stateTimer <= 0f)
        {
            NextState();
        }
    }

    void NextState()
    {
        switch (state)                              //This little timed system is so doesn't shoot instantly.
        {
            case State.SwingingSwordBeforeHit:         //'on hit'.
                state = State.SwingSwordAfterHit;
                float afterHitStateTime = 0.5f;
                stateTimer = afterHitStateTime;
                targetUnit.Damage(100);
                OnAnySwordHit?.Invoke(this, EventArgs.Empty);
                break;
            case State.SwingSwordAfterHit:
                OnSwordActionCompleted?.Invoke(this, EventArgs.Empty);
                ActionComplete();
                break;
        }
    }

    void AimAtTarget()
    {                                                                                             //Could also make a GetWorldPosition func on unit (returns transform.pos), then do targetUnit.GetWorldPosition - unit.GetWorldPosition().
        Vector3 aimDirection = (targetUnit.transform.position - transform.position).normalized;     //Gives us direction and magnitude (distance between two points), we only need direction so can use normalize.

        float rotateSpeed = 10f;  //So rotates faster.
        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * rotateSpeed);      //Make char face where he is going.
                                                                                                              //Lerp is applying smoothing between current transform.forward and moveDirection, the end result. Will move towards it each frame smoothly. Will move most at start, as transform is updating , so gap smaller and smaller, looks nice. For linear, would just store original transform.
    }

    public override string GetActionName()
    {
        return "Sword";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction { gridPosition = gridPosition, aiScore = 200 };    //So AI will ALWAYS prioritize this if it is possible.
    }

    public override List<GridPosition> GetValidActionGridPositionList()     //Returns list of VALID grid positions within one tile (swordMaxDistance).
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxSwordDistance; x <= maxSwordDistance; x++)    //Left to right, cycle thru valid grid positions. Eg if dist 1: -1, 0 and 1.
        {
            for (int z = -maxSwordDistance; z <= maxSwordDistance; z++)    //Same on z.
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);       //Cycled vals are an offset. Like -1, 0 and 1 if move dist of 1.
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;       //Take current pos and cycle thru all offsets. So if at 4,4 , can go 4,3, 4,4 , 4,5 etc...

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))      //if invalid (is negative or outside grid bounds), moves on to next.
                    continue;

                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))        //if there is no unit on that grid pos, continue next cycle of loop.
                    continue;

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (targetUnit.IsEnemy() == unit.IsEnemy())        //If target is on your 'team', ignore.
                    continue;


                validGridPositionList.Add(testGridPosition);        //If get thru all checks, grid pos is valid.
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        state = State.SwingingSwordBeforeHit;
        float beforeHitStateTime = 0.7f;           // Char swings sword, is in this state for 0.7 seconds. Then deal damage. Then in after swing state for 0.5 seconds.
        stateTimer = beforeHitStateTime;

        OnSwordActionStarted?.Invoke(this, EventArgs.Empty);
        ActionStart(onActionComplete);       //Passing in clear busy delegate.
    }


    public int GetMaxSwordDistance() => maxSwordDistance;


}
