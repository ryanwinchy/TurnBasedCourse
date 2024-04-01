using UnityEngine;
using System;

public class Unit : MonoBehaviour
{

    [SerializeField] const int ACTION_POINTS_MAX = 2;

    public static event EventHandler OnAnyActionPointsChanged;    //event declaration. Static means fires on ANY unit, not a specific instance.
                                                                //We made specific event for action points changing. If just used 'turn changed event' , danger of event going to UI before action points actually updating or vise versa. So would have some 0 points bugs.
    GridPosition gridPosition;         //Current pos

    BaseAction[] baseActionArray;
    MoveAction moveAction;
    SpinAction spinAction;

    int actionPoints = ACTION_POINTS_MAX;


    private void Awake()
    {
        baseActionArray = GetComponents<BaseAction>();   //Stores all components attached to this unit that derive (inherit from) base action.So spin and move actions for eg.
        moveAction = GetComponent<MoveAction>();
        spinAction = GetComponent<SpinAction>();
    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position); //Get grid pos from units current position.
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;        //Subscribe to turn changing event.
    }

    private void Update()
    {


        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position); //Get grid pos from units current position.

        if (newGridPosition != gridPosition)     //Unit has moved.
        {
            LevelGrid.Instance.UnitMoveGridPosition(this, gridPosition, newGridPosition);
            gridPosition = newGridPosition;
        }
            
    }

    public MoveAction GetMoveAction() => moveAction;
    public SpinAction GetSpinAction() => spinAction;

    public GridPosition GetGridPosition() => gridPosition;

    public BaseAction[] GetBaseActionArray() => baseActionArray;


    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if (CanSpendActionPointsToTakeAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointsCost());
            return true;
        }
        else
            return false;
    }

    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction)  //tests if we can spend the action points. baseAction here will be a child like spinAction or moveAction. Default is 1 from base, but sometimes overriden to be 2 or more points.
    {
        if (actionPoints >= baseAction.GetActionPointsCost())
            return true;
        else
            return false;
    }

    void SpendActionPoints(int amountToSpend)
    {
        actionPoints -= amountToSpend;

        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);   //Fire event.
    }

    public int GetActionPoints() => actionPoints;

    void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        actionPoints = ACTION_POINTS_MAX;

        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);   //Fire event.
    }
}
