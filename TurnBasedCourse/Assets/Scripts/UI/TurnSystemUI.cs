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
    [SerializeField] GameObject enemyTurnVisualGameObject;

    private void Start()
    {
        UpdateTurnText();
        UpdateEnemyTurnVisual();     //Run right away, so will turn off as player starts.
        UpdateEndTurnVisual();

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;

        endTurnButton.onClick.AddListener(() =>                          //Same as setting on click on button in editor.
        {
            TurnSystem.Instance.NextTurn();
        });
    }

    void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateTurnText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnVisual();
    }

    void UpdateTurnText()
    {
        turnNumber.text = "Turn " + TurnSystem.Instance.GetTurnNumber();
    }

    void UpdateEnemyTurnVisual() => enemyTurnVisualGameObject.SetActive(!TurnSystem.Instance.IsPlayerTurn());    //Turn on visual when enemy turn.

    void UpdateEndTurnVisual() => endTurnButton.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn());     //Turn on end turn button only in player turn.

}
