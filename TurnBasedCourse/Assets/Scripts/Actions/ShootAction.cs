using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShootAction : BaseAction
{
    public event EventHandler<OnShootEventArgs> OnShoot;    //event for shoot animation trigger.

    public class OnShootEventArgs : EventArgs    //Another way to send data on the event. This method is good if a lot of data.
    {
        public Unit targetUnit;
        public Unit shootingUnit;
    }

    enum State { Aiming, Shooting, Cooloff }      //Basic state machine to show states of the shoot action within this script.
    State state;
    float stateTimer;

    Unit targetUnit;

    bool canShootBullet;

    int maxShootDistance = 7;

    private void Update()
    {
        if (!isActive)
            return;

        stateTimer -= Time.deltaTime;   //So happens for all states.

        switch (state)
        {
            case State.Aiming:
                AimAtTarget();
                break;
            case State.Shooting:
                if (canShootBullet)
                {
                    Shoot();
                    canShootBullet = false;
                }
                break;
            case State.Cooloff:
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
            case State.Aiming:
                state = State.Shooting;
                float shootingStateTime = 0.1f;
                stateTimer = shootingStateTime;
                break;
            case State.Shooting:
                state = State.Cooloff;
                float cooloffStateTime = 0.5f;
                stateTimer = cooloffStateTime;
                break;
            case State.Cooloff:
                ActionComplete();     //Base func, avoids repeated code on all actions. It is setting this action to inactive, and calling ClearBusy delegate, stored in onActionComplete var.
                break;
        }
    }

    void Shoot()
    {
        OnShoot?.Invoke(this, new OnShootEventArgs { targetUnit = targetUnit, shootingUnit = unit});      //Fire event for animator to listen to.
        
        targetUnit.Damage(40);     //For now standard 40. could have different weapons, get the component and have different damages.
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
        return "Shoot";
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxShootDistance; x <= maxShootDistance; x++)    //Left to right, cycle thru valid grid positions. Eg if dist 1: -1, 0 and 1.
        {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++)    //Same on z.
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);       //Cycled vals are an offset. Like -1, 0 and 1 if move dist of 1.
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;       //Take current pos and cycle thru all offsets. So if at 4,4 , can go 4,3, 4,4 , 4,5 etc...

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))      //if invalid (is negative or outside grid bounds), moves on to next.
                    continue;

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);       //Mathf as want the positive only, not the negative.
                if (testDistance > maxShootDistance)        //This is creating a triangle of range instead of a square. Saying add up x and y ( like 5 to right 5 up), will be out of range as max is 7.
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
        
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);   //Takes in the valid grid position you selected after choosing action, saves as target unit.

        state = State.Aiming;  //Defaults to aiming state . Basic mini state machine within this script, simple.
        float aimingStateTime = 1f;
        stateTimer = aimingStateTime;

        canShootBullet = true;

        ActionStart(onActionComplete);                 //Base function, sets this action active, and stores ClearBusy delegate in onAction complete var.

    }


    public Unit GetTargetUnit() => targetUnit;

    public int GetMaxShootDistance() => maxShootDistance;


}