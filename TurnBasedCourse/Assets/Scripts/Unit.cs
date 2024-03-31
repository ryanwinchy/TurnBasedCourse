using UnityEngine;

public class Unit : MonoBehaviour
{

    GridPosition gridPosition;         //Current pos

    BaseAction[] baseActionArray;
    MoveAction moveAction;
    SpinAction spinAction;


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
}
