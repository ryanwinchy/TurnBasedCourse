using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    float timer;

    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;      //Subscribe to on turn changed event.
    }
    private void Update()
    {
        if (TurnSystem.Instance.IsPlayerTurn()) //If players turn, then update does nothing. Only act if enemies turn.
            return;

        timer -= Time.deltaTime;

        if (timer < 0 )
            TurnSystem.Instance.NextTurn();         //After timer, go back to player turn.

    }

    void TurnSystem_OnTurnChanged(object sender, EventArgs e)  //on turn change event.
    {
        timer = 3f;            //Change back to player turn in 3 seconds.
    }
}
