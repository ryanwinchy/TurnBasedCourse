using System;
using UnityEngine;

public class Unit : MonoBehaviour
{

    [SerializeField] bool isEnemy;

    [SerializeField] const int ACTION_POINTS_MAX = 2;

    public static event EventHandler OnAnyActionPointsChanged;    //event declaration. Static means fires on ANY unit, not a specific instance.
                                                                  //We made specific event for action points changing. If just used 'turn changed event' , danger of event going to UI before action points actually updating or vise versa. So would have some 0 points bugs.
    
    public static event EventHandler OnAnyUnitSpawned;       //Useful for enemy AI. Static, so when this happens on ANY unit, not a specific instance.
    public static event EventHandler OnAnyUnitDead;


    GridPosition gridPosition;         //Current pos

    HealthSystem healthSystem;

    BaseAction[] baseActionArray;

    int actionPoints = ACTION_POINTS_MAX;


    private void Awake()
    {
        baseActionArray = GetComponents<BaseAction>();   //Stores all components attached to this unit that derive (inherit from) base action.So spin and move actions for eg.
        healthSystem = GetComponent<HealthSystem>();
    }

    private void Start()
    {


        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position); //Get grid pos from units current position.

        Debug.Log(gridPosition);

        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;        //Subscribe to turn changing event.
        healthSystem.OnDead += HealthSystem_OnDead;     //Subscribe to on dead.

        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);    //Fires unit spawned event.
    }

    private void Update()
    {


        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position); //Get grid pos from units current position.

        if (newGridPosition != gridPosition)     //Unit has moved.
        {
            GridPosition originalGridPosition = gridPosition;

            gridPosition = newGridPosition;
            LevelGrid.Instance.UnitMoveGridPosition(this, originalGridPosition, newGridPosition);     //This func updates unit pos, and fires unit moved event, so pos change (above line) HAS to happen first. Bug fixed...
        }

    }
                                                             
    public T GetAction<T>() where T : BaseAction         //T must extend base action, so much be shootAction, SpinAction etc...
    {                                                            ////////////////This func gets any specified action type. MUCH better than loads of functions for getting each action, refs, and get components. That wouldnt scale if had 50. Now instead of unit.GetMoveAction() it's unit.GetAction<MoveAction>().
        foreach (BaseAction baseAction in baseActionArray) //Cycles thru all actions attached to this unit. 
        {
            if (baseAction is T)             //If action is of type T.
                return (T)baseAction;       //Return action (cast to specific type T).
        }
        return null;
    }

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
        if ((IsEnemy() && !TurnSystem.Instance.IsPlayerTurn()) || !IsEnemy() && TurnSystem.Instance.IsPlayerTurn())  //if enemy and enemy turn, or player and player turn reset action points.
        {
            actionPoints = ACTION_POINTS_MAX;

            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);   //Fire event.
        }

    }

    public bool IsEnemy() => isEnemy;

    public void Damage(int damageAmount)
    {
        healthSystem.TakeDamage(damageAmount);
    }


    void HealthSystem_OnDead(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);        //Remove from grid.

        Destroy(gameObject);

        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);    //Fires unit spawned event.
    }

    public float GetHealthNormalized() => healthSystem.GetHealthNormalized();

}
