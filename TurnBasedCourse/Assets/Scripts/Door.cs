using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Door : MonoBehaviour, IInteractable
{

    GridPosition gridPosition;
    Animator animator;

    [SerializeField] bool isOpen;

    Action onInteractionComplete;       //Storing delegated function.

    float timer;
    bool isActive;

    public event EventHandler OnDoorOpened;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);  //Gets grid pos of where you placed the door in world.

        LevelGrid.Instance.SetInteractableAtGridPosition(this, gridPosition);       //Sets door at that grid position. So grid knows about door.

        if (isOpen)
            OpenDoor();
        else
            CloseDoor();
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
    public void Interact(Action onInteractionComplete)
    {
        this.onInteractionComplete = onInteractionComplete;    //Store function given as param. Function just sets action to complete on InteractAction script. Allows us to do it after a timer basically.
        isActive = true;
        timer = 0.5f;

        if (isOpen)
            CloseDoor();
        else
            OpenDoor();
    }

    void OpenDoor()
    {
        isOpen = true;
        animator.SetBool("IsOpen", isOpen);
        Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, true);

        OnDoorOpened?.Invoke(this, EventArgs.Empty);
    }

    void CloseDoor()
    {
        isOpen = false;
        animator.SetBool("IsOpen", isOpen);
        Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, false);
    }



}
