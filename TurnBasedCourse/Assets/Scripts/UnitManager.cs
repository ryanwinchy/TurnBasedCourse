using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance { get; private set; }          //Singleton. Any class can read, only this class can set. Properties are Pascal case.


    List<Unit> unitList;
    List<Unit> friendlyUnitList;
    List<Unit> enemyUnitList;

    private void Awake()
    {
        if (Instance != null)          //Singleton. If duplicates, destroys the dupe. So one single instance.
        {
            Destroy(gameObject);
            return;
        }
        else
            Instance = this;


        unitList = new List<Unit>();
        friendlyUnitList = new List<Unit>();
        enemyUnitList = new List<Unit>();


    }

    private void Start()          //This start could run after the Start() on unit which spawns the unit and fires event. So, make SURE it runs first in script execution order settings.
    {
        Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;     //Event listeners.
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
    }


    void Unit_OnAnyUnitSpawned(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;    //As sender is the unit.

        unitList.Add(unit);

        if (unit.IsEnemy())
        {
            enemyUnitList.Add(unit);
        }
        else
        {
            friendlyUnitList.Add(unit);
        }
    }

    void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;    //As sender is the unit.

        unitList.Remove(unit);

        if (unit.IsEnemy())
        {
            enemyUnitList.Remove(unit);
        }
        else
        {
            friendlyUnitList.Remove(unit);
        }
    }

    public List<Unit> GetUnitList() => unitList;
    public List<Unit> GetFriendlyList() => friendlyUnitList;
    public List<Unit> GetEnemyList() => enemyUnitList;


}
