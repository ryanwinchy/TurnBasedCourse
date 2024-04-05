using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TurnSystem : MonoBehaviour
{
    public static TurnSystem Instance { get; private set; }

    public event EventHandler OnTurnChanged;

    int turnNumber = 1;
    bool isPlayerTurn = true;   //True by default so player takes first turn.

    
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

    public void NextTurn()
    {
        turnNumber++;

        isPlayerTurn = !isPlayerTurn;  //Each turn it flips (from player turn to not player turn).

        OnTurnChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetTurnNumber() => turnNumber;

    public bool IsPlayerTurn() => isPlayerTurn;




}
