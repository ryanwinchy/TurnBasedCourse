using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelScripting : MonoBehaviour
{

    [SerializeField] List<GameObject> hider1List;
    [SerializeField] List<GameObject> hider2List;

    [SerializeField] List<GameObject> hiddenUnits1;
    [SerializeField] List<GameObject> hiddenUnits2;

    [SerializeField] Door door1;
    [SerializeField] Door door2;


    private void Start()
    {
        SetActiveGameObjectList(hider1List, true);
        SetActiveGameObjectList(hider2List, true);
        SetActiveGameObjectList(hiddenUnits1, false);
        SetActiveGameObjectList(hiddenUnits2, false);

        door1.OnDoorOpened += Door_OnDoorOpened;
        door2.OnDoorOpened += Door2_OnDoorOpened;
    }

    private void Door_OnDoorOpened(object sender, EventArgs e)
    {
        SetActiveGameObjectList(hider1List, false);
        SetActiveGameObjectList(hiddenUnits1, true);
    }

    private void Door2_OnDoorOpened(object sender, EventArgs e)
    {
        SetActiveGameObjectList(hider2List, false);
        SetActiveGameObjectList(hiddenUnits2, true);
    }


    void SetActiveGameObjectList(List<GameObject> hiderList, bool show)
    {
        foreach (GameObject hider in hiderList)
        {
            hider.SetActive(show);
        }
    }
}
