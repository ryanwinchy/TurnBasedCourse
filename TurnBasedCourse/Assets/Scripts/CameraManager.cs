using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    [SerializeField] GameObject actionCameraGameObject;

    private void Start()
    {
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;      //Sub to event. Static event, so called ON ANY action, not a specific instance.
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;      //Sub to event. Static event, so called ON ANY action, not a specific instance.
    }

    void ShowActionCamera() => actionCameraGameObject.SetActive(true);      //Turning this off and on changes the view, as that cinemachine cam has higher priority.

    void HideActionCamera() => actionCameraGameObject.SetActive(false);

    void BaseAction_OnAnyActionStarted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:       //If sender is a shoot action, show action camera. Incredibly powerful, can easily add loads of cam angles for diff actions!
                Unit shooterUnit = shootAction.GetUnit();         //CAMERA POSITIONING FOR ACTION SHOT.
                Unit targetUnit = shootAction.GetTargetUnit();
                
                Vector3 cameraCharacterHeight = Vector3.up * 1.7f;     //Move cam up 1.7f from character.

                Vector3 shootDirection = (targetUnit.transform.position - shooterUnit.transform.position).normalized;     //Normalized as is direction, only need magnitude not distance.

                float shoulderOffsetAmount = 0.5f;
                Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * shootDirection * shoulderOffsetAmount;      //so camera is rotated a bit , looks like over shoulder instead of straight at enemy.

                Vector3 actionCameraPosition = shooterUnit.transform.position + cameraCharacterHeight + shoulderOffset + (shootDirection * -1);    //End brackets push it backwards a bit so can see shooter unit as well.

                actionCameraGameObject.transform.position = actionCameraPosition;
                actionCameraGameObject.transform.LookAt(targetUnit.transform.position + cameraCharacterHeight);
                ShowActionCamera();
                break;
        }
    }

    void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:       //If sender is a shoot action, show action camera. Incredibly powerful, can easily add loads of cam angles for diff actions!
                HideActionCamera();
                break;
        }
    }


}
