using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField] Transform actionButtonPrefab;         //Can use Transform or GameObject doesnt matter.
    [SerializeField] Transform actionButtonContainerTransform;

    List<ActionButtonUI> actionButtonUIList;

    private void Awake()
    {
        actionButtonUIList = new List<ActionButtonUI>();
    }

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;   //Subscribe to event.
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;   //Subscribe to event.

        CreateUnitActionButtons();
        UpdateSelectedVisual();
    }


    void CreateUnitActionButtons()
    {
        foreach (Transform buttonTransform in actionButtonContainerTransform)    //Cycle thru children transforms in the parent (transform is same as game object here).
        {
            Destroy(buttonTransform.gameObject);
        }

        actionButtonUIList.Clear();


        Unit selectedUnit = UnitActionSystem.Instance?.GetSelectedUnit();

        foreach (BaseAction baseAction in selectedUnit.GetBaseActionArray())  //Cycle thru the actions on the unit (that inherit from base action), like spin, move action scripts.
        {
            Transform actionButtonTransform = Instantiate(actionButtonPrefab, actionButtonContainerTransform);   //Instantiate prefab in container.
            ActionButtonUI actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>();

            actionButtonUI.SetBaseAction(baseAction);  //For each button created, assigns the base action from the array of all action types of the unit.

            actionButtonUIList.Add(actionButtonUI);   //so we have list of active UI buttons.
        }
    }

    void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)    //Standard event sub, param of sender and no event arguments.
    {
        CreateUnitActionButtons();
        UpdateSelectedVisual();
    }

    void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)    //Standard event sub, param of sender and no event arguments.
    {
        UpdateSelectedVisual();
    }

    public void UpdateSelectedVisual()
    {
        foreach (ActionButtonUI actionButtonUI in actionButtonUIList)
        {
            actionButtonUI.UpdateSelectedVisual();
        }
    }

}
