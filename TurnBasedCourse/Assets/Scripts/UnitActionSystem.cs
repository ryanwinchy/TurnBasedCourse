using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set; }          //Singleton. Any class can read, only this class can set. Properties are Pascal case.

    public event EventHandler OnSelectedUnitChanged;    //Defines event for when we change unit selection.

    [SerializeField] LayerMask unitLayer;
    [SerializeField] Unit selectedUnit;


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
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (TryHandleUnitSelection())       //If we selected a unit, skip to next update frame, dont move unit this frame.
                return;

            selectedUnit.SetTargetPosition(MouseWorld.GetPosition());
        }



    }


    bool TryHandleUnitSelection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);   //Takes screen pos (input.mousepos) and makes ray from camera to it.
                                                                       //out just means function wont write to raycastHit variable, so var doesnt have to be set up.
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayer))        //Returns true if hits any collider and false if not. Physics always deals with colliders.
        {
            if (raycastHit.collider.gameObject.TryGetComponent<Unit>(out Unit unit))   //If gets component of Unit, assign to unit var, nested code runs if not null.
            {                                                                         //Equivalent to normal GetComponent then an if != null.
                SetSelectedUnit(unit);
                return true;
            }
        }

        return false;
                                                                                                             
    }

    void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;

        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);      //Very compact way to fire event. Does null check to see if has subscribers with the ?. If it does, fires event. 'this' says sender is this script, we're not sending any info so EventArgs.empty.
                                                                   //Equivalent to:       if (OnSelectedUnitChanged != null)
                                                                                                //OnSelectedUnitChanged(this, EventArgs.Empty);
    }

    public Unit GetSelectedUnit() => selectedUnit;     //Exposes currently selected unit while keeping it private.


}
