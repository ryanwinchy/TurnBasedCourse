using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBusyUI : MonoBehaviour
{
    [SerializeField] GameObject busyVisual;


    private void Start()
    {
        UnitActionSystem.Instance.OnBusyChanged += UnitActionSystem_OnBusyChanged;   //Subscribe to event.
        HideVisual();
    }

    void UnitActionSystem_OnBusyChanged(object sender, bool isBusy)
    {
        if (isBusy)
            ShowVisual();
        else
            HideVisual();
    }
    void ShowVisual()
    {
        busyVisual.SetActive(true);
    }


    void HideVisual()
    {
        busyVisual.SetActive(false);
    }



}
