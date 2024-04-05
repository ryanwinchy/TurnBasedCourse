using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class UnitWorldUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI actionPointsText;
    [SerializeField] Unit unit;
    [SerializeField] Image healthBarImage;
    [SerializeField] HealthSystem healthSystem;

    private void Start()
    {
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;     //Subscribe to event.
        healthSystem.OnDamaged += HealthSystem_OnDamaged;      //Subscribe to event.

        UpdateActionPointsText();
        UpdateHealthBar();
    }

    void UpdateActionPointsText()
    {
        actionPointsText.text = unit.GetActionPoints().ToString();
    }

    void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPointsText();
    }

    void HealthSystem_OnDamaged(object sender, EventArgs e)
    {
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        healthBarImage.fillAmount = healthSystem.GetHealthNormalized();
    }

}
