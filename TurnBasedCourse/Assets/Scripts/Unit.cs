using UnityEngine;

public class Unit : MonoBehaviour
{

    GridPosition gridPosition;         //Current pos

    MoveAction moveAction;

    private void Awake()
    {
        moveAction = GetComponent<MoveAction>();
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

    public GridPosition GetGridPosition() => gridPosition;
}
