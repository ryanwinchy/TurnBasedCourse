using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ActionButtonUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textMeshPro;
    [SerializeField] Button button;
    [SerializeField] GameObject selectedVisual;

    BaseAction baseAction;


    private void Awake()
    {
    }

    public void SetBaseAction(BaseAction baseAction)
    {
        this.baseAction = baseAction;
        textMeshPro.text = baseAction.GetActionName().ToUpper();       //Calling overriden GetAction names.

        button.onClick.AddListener(() =>                          //Same as setting on click on button in editor. We instantiate button so must use code.
        {
            UnitActionSystem.Instance.SetSelectedAction(baseAction);
        });         
    }

    public void UpdateSelectedVisual()
    {
        BaseAction selectedBaseAction = UnitActionSystem.Instance.GetSelectedAction();  //Get currently selected action.

        selectedVisual.SetActive(selectedBaseAction == baseAction);   //If this buttons action is the currently selected, set visual active.
    }


}
