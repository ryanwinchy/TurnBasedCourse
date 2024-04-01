using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystemUI : MonoBehaviour
{

    [SerializeField] Button endTurnButton;
    [SerializeField] TextMeshProUGUI turnNumber;


    private void Start()
    {
        UpdateTurnText();

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;

        endTurnButton.onClick.AddListener(() =>                          //Same as setting on click on button in editor.
        {
            TurnSystem.Instance.NextTurn();
        });
    }

    void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateTurnText();
    }

    void UpdateTurnText()
    {
        turnNumber.text = "Turn " + TurnSystem.Instance.GetTurnNumber();
    }

}
