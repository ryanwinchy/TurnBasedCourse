using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class MouseWorld : MonoBehaviour
{
    static MouseWorld instance;
    [SerializeField] LayerMask mousePlaneLayerMask;

    private void Awake()
    {
        instance = this;     //This should probably be singleton.
    }

    public static Vector3 GetPosition()     //Static so this function belongs to the class itself instead of an instance.
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());   //Takes screen pos (input.mousepos) and makes ray from camera to it.
                                                                       //out just means function wont write to raycastHit variable, so var doesnt have to be set up.
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.mousePlaneLayerMask);        //Returns true if hits any collider and false if not. Physics always deals with colliders.
                                                                                                                //Use max value, as we dont want our ray to have a max distance. Layer mask so only collides with the mosue plane layer (the ground).
        return raycastHit.point;
    }
}
