using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField] Transform actionButtonPrefab;         //Can use Transform or GameObject doesnt matter.
    [SerializeField] Transform actionButtonContainerTransform;
    [SerializeField] TextMeshProUGUI actionPointsText;

    List<ActionButtonUI> actionButtonUIList;

    private void Awake()
    {
        actionButtonUIList = new List<ActionButtonUI>();
    }

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;   //Subscribe to event.
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;   //Subscribe to event.
        UnitActionSystem.Instance.OnActionStarted += UnitActionSystem_OnActionStarted;   //Subscribe to event.
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;   //Guarantees we update action points just after action points change. No danger of UI event reacting before its actually updated.



        CreateUnitActionButtons();
        UpdateSelectedVisual();
        UpdateActionPoints();
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
        UpdateActionPoints();
    }

    void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)    //Standard event sub, param of sender and no event arguments.
    {
        UpdateSelectedVisual();
    }

    void UnitActionSystem_OnActionStarted(object sender, EventArgs e)      //Standard event sub, param of sender and no event arguments.
    {
        UpdateActionPoints();
    }

    public void UpdateSelectedVisual()
    {
        foreach (ActionButtonUI actionButtonUI in actionButtonUIList)
        {
            actionButtonUI.UpdateSelectedVisual();
        }
    }

    void UpdateActionPoints()
    {
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();


        actionPointsText.text = "Action Points: " + selectedUnit.GetActionPoints();
    }

    void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)   //Guarantees we update action points as action points change. No danger of UI event reacting before its actually updated.
    {
        UpdateActionPoints();
    }

}
