using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set; }          //Singleton. Any class can read, only this class can set. Properties are Pascal case.

    public event EventHandler OnSelectedUnitChanged;    //Defines event for when we change unit selection.
    public event EventHandler OnSelectedActionChanged;    //Defines event for when we change action selection.
    public event EventHandler<bool> OnBusyChanged;    //Defines event for when we become busy or stop being busy.
    public event EventHandler OnActionStarted;

    [SerializeField] LayerMask unitLayer;
    [SerializeField] Unit selectedUnit;        //Default is one you put in editor, then changes if select a diff one.

    BaseAction selectedAction;

    bool isBusy;


    private void Awake()
    {
        if (Instance != null)          //Singleton. If duplicates, destroys the dupe. So one single instance.
        {
            Destroy(gameObject);
            return;
        }
        else
            Instance = this;
    }

    private void Start()
    {
        SetSelectedUnit(selectedUnit);      //Select the default unit.
    }


    private void Update()
    {
        if (isBusy)      //If action already running, don't do another.
            return;

        if (EventSystem.current.IsPointerOverGameObject())       //If mouse is over the UI, don't run the movement and action.Without this, will move as you click move if valid spot.
            return;


        if (TryHandleUnitSelection())       //If we selected a unit, skip to next update frame, dont move unit this frame.
            return;


        HandleSelectedAction();

    }

    void HandleSelectedAction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

            if (selectedAction.IsValidActionGridPosition(mouseGridPosition))    //Uses base, so works for all children actions.
            {
                if (selectedUnit.TrySpendActionPointsToTakeAction(selectedAction))   //Checks points, if can take action spends points and takes action.
                {
                    SetBusy();                                                 
                    selectedAction.TakeAction(mouseGridPosition, ClearBusy);

                    OnActionStarted?.Invoke(this, EventArgs.Empty);      //Very compact way to fire event.

                }
            }



        }
    }

    void SetBusy()
    {
        isBusy = true;
        OnBusyChanged?.Invoke(this, isBusy);      //Very compact way to fire event.
    }
    void ClearBusy()
    {
        isBusy = false;
        OnBusyChanged?.Invoke(this, isBusy);      //Very compact way to fire event.
    }
    bool TryHandleUnitSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);   //Takes screen pos (input.mousepos) and makes ray from camera to it.
                                                                           //out just means function wont write to raycastHit variable, so var doesnt have to be set up.
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayer))        //Returns true if hits any collider and false if not. Physics always deals with colliders.
            {
                if (raycastHit.collider.gameObject.TryGetComponent<Unit>(out Unit unit))   //If gets component of Unit, assign to unit var, nested code runs if not null.
                {                                                                         //Equivalent to normal GetComponent then an if != null.
                    if (unit == selectedUnit)  //Unit already selected. Returns false, and just takes the action.
                    {
                        return false;
                    }
                    SetSelectedUnit(unit);
                    return true;
                }
            }
        }

        return false;

    }

    void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        SetSelectedAction(unit.GetMoveAction());     //When select a unit, defaults to move action.

        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);      //Very compact way to fire event. Does null check to see if has subscribers with the ?. If it does, fires event. 'this' says sender is this script, we're not sending any info so EventArgs.empty.
                                                                   //Equivalent to:       if (OnSelectedUnitChanged != null)
                                                                   //OnSelectedUnitChanged(this, EventArgs.Empty);
    }

    public void SetSelectedAction(BaseAction baseAction)
    {
        selectedAction = baseAction;    //Set selected action to a given baseAction (spinAction and moveAction are base actions).

        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);      //Very compact way to fire event. Does null check to see if has subscribers with the ?. If it does, fires event. 'this' says sender is this script, we're not sending any info so EventArgs.empty.
                                                                     //Equivalent to:       if (OnSelectedUnitChanged != null)
                                                                     //OnSelectedUnitChanged(this, EventArgs.Empty);
    }

    public Unit GetSelectedUnit() => selectedUnit;     //Exposes currently selected unit while keeping it private.

    public BaseAction GetSelectedAction() => selectedAction;


}
