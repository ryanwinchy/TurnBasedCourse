using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAction : BaseAction
{

    [SerializeField] Transform grenadeProjectilePrefab;

    int maxThrowDistance = 7;

    private void Update()
    {
        if (!isActive)
            return;

       
    }
    public override string GetActionName()
    {
        return "Grenade";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction { gridPosition = gridPosition, aiScore = 0 };   //Quick way to setup new class, pass in its vars.
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxThrowDistance; x <= maxThrowDistance; x++)    //Left to right, cycle thru valid grid positions. Eg if dist 1: -1, 0 and 1.
        {
            for (int z = -maxThrowDistance; z <= maxThrowDistance; z++)    //Same on z.
            {
                GridPosition offsetGridPosition = new GridPosition(x, z, 0);       //Cycled vals are an offset. Like -1, 0 and 1 if move dist of 1.
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;       //Take current pos and cycle thru all offsets. So if at 4,4 , can go 4,3, 4,4 , 4,5 etc...

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))      //if invalid (is negative or outside grid bounds), moves on to next.
                    continue;

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);       //Mathf as want the positive only, not the negative.
                if (testDistance > maxThrowDistance)        //This is creating a triangle of range instead of a square. Saying add up x and y ( like 5 to right 5 up), will be out of range as max is 7.
                    continue;


                validGridPositionList.Add(testGridPosition);        //If get thru all checks, grid pos is valid.
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        Transform grenadeProjectileTransform = Instantiate(grenadeProjectilePrefab, unit.transform.position, Quaternion.identity);
        GrenadeProjectile newGrenadeProjectile = grenadeProjectileTransform.GetComponent<GrenadeProjectile>();

        newGrenadeProjectile.Setup(gridPosition, OnGrenadeBehaviourComplete);    //Send target grid pos and delegate function so grenade itself can call action complete once exploded, so cant spam grenades. Action completes when grenade is exploded.

        ActionStart(onActionComplete);
    }

    void OnGrenadeBehaviourComplete()
    {
        ActionComplete();
    }

}
