using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IInteractable
{

    void Interact(Action onInteractionComplete);       //Must pass in delegate function to do when interaction is complete. It is the 'OnActionComplete' function which means we can add a timed component in the action script to finish the action rather than instant.


}
