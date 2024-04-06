using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{

    enum State { WaitingForEnemyTurn, TakingTurn, Busy }        //Basic state machine within this script.

    State state;

    float timer;

    private void Awake()
    {
        state = State.WaitingForEnemyTurn;    //Defaults to waiting for turn.
    }

    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;      //Subscribe to on turn changed event.
    }
    private void Update()
    {
        if (TurnSystem.Instance.IsPlayerTurn()) //If players turn, then update does nothing. Only act if enemies turn.
            return;

        switch (state)
        {
            case State.WaitingForEnemyTurn:
                break;
            case State.TakingTurn:
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    if (TryTakeEnemyAIAction(SetStateTakingTurn))
                        state = State.Busy;
                    else                                    //No more enemies have actions they can take.
                        TurnSystem.Instance.NextTurn();         //Go back to player turn.
                }

                break;
            case State.Busy:
                break;
        }



    }

    void SetStateTakingTurn()
    {
        timer = 0.5f;
        state = State.TakingTurn;
    }

    void TurnSystem_OnTurnChanged(object sender, EventArgs e)  //on turn change event.
    {
        if (!TurnSystem.Instance.IsPlayerTurn())         //If enemy turn.
        {
            state = State.TakingTurn;
            timer = 3f;         //Change back to player turn in 3 seconds.
        }

    }

    bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)  //Passing in function. Action here means delegate function, with no return and no params. Func passed is 'set taking turn state'.
    {
        foreach (Unit enemyUnit in UnitManager.Instance.GetEnemyList())     //Cycle thru all currently alive enemy units.
        {
            if (TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete))
            {
                return true;
            }

        }

        return false;
    }

    bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete)      //Func passed is 'set taking turn state'.
    {
        EnemyAIAction bestEnemyAIAction = null;
        BaseAction bestBaseAction = null;

        foreach(BaseAction baseAction in enemyUnit.GetBaseActionArray())        //Cycle thru all actions attached to enemy unit. Move, shoot, spin. Get best action and best ai score of the action (like for move, which move has highest score which we set to most in range enemies), or for shoot, hitting targets has best ai score of all valid shooting grid positions.
        {                                                                          //based on rules we set, if enemy within range it should shoot them as priority (we gave 100 score), if not should move to pos with most valid targets, then shoot, if cant do anything else, should spin. If cant spin, do nothing.
            if (!enemyUnit.CanSpendActionPointsToTakeAction(baseAction))           //cannot afford this action.
                continue;

            if (bestEnemyAIAction == null)      //If first one testing.
            {
                bestEnemyAIAction = baseAction.GetBestEnemyAIAction();     //for first child action tested, like shoot, get best enemy AI action for shoot (will test all valid shooting range, the best (enemy hits) have ai score of 100 so would win.
                bestBaseAction = baseAction;
            }
            else          //Have a previous best.
            {
                EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();

                if (testEnemyAIAction != null && testEnemyAIAction.aiScore > bestEnemyAIAction.aiScore)
                {
                    bestEnemyAIAction = testEnemyAIAction;     
                    bestBaseAction = baseAction;
                }
            }
        }

        if (bestEnemyAIAction != null && enemyUnit.TrySpendActionPointsToTakeAction(bestBaseAction))        //If there is a best action, try to spend points and take the action.
        {
            bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyAIActionComplete);           //Given action is in the onEnemyAIActionComplete delegate, in this 'set taking turn state'.
            return true;
        }
        else
            return false;


    }

}
