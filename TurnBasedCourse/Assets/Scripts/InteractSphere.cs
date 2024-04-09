using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractSphere : MonoBehaviour, IInteractable
{
    [SerializeField] Material greenMaterial;
    [SerializeField] Material redMaterial;
    [SerializeField] MeshRenderer meshRenderer;

    GridPosition gridPosition;

    bool isGreen;

    Action onInteractionComplete;       //Storing delegated function.

    float timer;
    bool isActive;

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);  //Gets grid pos of where you placed the door in world.

        LevelGrid.Instance.SetInteractableAtGridPosition(this, gridPosition);       //Sets door at that grid position. So grid knows about door.

        SetColourGreen();
    }

    private void Update()
    {
        if (!isActive)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            isActive = false;
            onInteractionComplete();    //Run stored delegate function.
        }

    }
    void SetColourGreen()
    {
        isGreen = true;
        meshRenderer.material = greenMaterial;
    }

    void SetColourRed()
    {
        isGreen = false;
        meshRenderer.material = redMaterial;

    }

    public void Interact(Action onInteractionComplete)
    {
        this.onInteractionComplete = onInteractionComplete;    //Store function given as param. Function just sets action to complete on InteractAction script. Allows us to do it after a timer basically.
        isActive = true;
        timer = 0.5f;

        if (isGreen)
            SetColourRed();
        else
            SetColourGreen();
    }
}
