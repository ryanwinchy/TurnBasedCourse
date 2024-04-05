using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectedVisual : MonoBehaviour
{

    [SerializeField] Unit unit;

    MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;    //Listening for event. When fired runs the func.
        
        UpdateVisual();
    }

    void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs empty)    //Standard event, no info sent so eventargs empty, and we know object that sent it.
    {
        UpdateVisual();
    }

    void UpdateVisual()
    {
        if (unit == UnitActionSystem.Instance.GetSelectedUnit())       //If this prefabs unit is 'selected', show the object.
            meshRenderer.enabled = true;
        else
            meshRenderer.enabled = false;
    }

    private void OnDestroy()          //Monobehaviour, called when this game object destroyed.
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged -= UnitActionSystem_OnSelectedUnitChanged;    //Unsubscribe when selected unit destroyed (which happens when unit destroyed).
    }


}
