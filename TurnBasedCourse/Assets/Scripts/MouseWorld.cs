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

    public static Vector3 GetPosition()     //Static so this function belongs to the class itself instead of a specific instance.   WE USED THIS BEFORE FUNC BELOW BEFORE MULTI FLOOR. USE THIS AS REF, MUCH SIMPLER. AM LOST WITH THING BELOW, IS SO ABSTRACT.
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());   //Takes screen pos (input.mousepos) and makes ray from camera to it.
                                                                       //out just means function wont write to raycastHit variable, so var doesnt have to be set up.
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.mousePlaneLayerMask);        //Returns true if hits any collider and false if not. Physics always deals with colliders.
                                                                                                                //Use max value, as we dont want our ray to have a max distance. Layer mask so only collides with the mosue plane layer (the ground).
        return raycastHit.point;
    }

    public static Vector3 GetPositionHitVisible()  //New function for multi floor so checks the position you select is actually visible.
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());   
                                                                                                 
        RaycastHit[] raycastHitArray = Physics.RaycastAll(ray, float.MaxValue, instance.mousePlaneLayerMask);

        System.Array.Sort(raycastHitArray, (RaycastHit raycastA, RaycastHit raycastHitB) => { return Mathf.RoundToInt(raycastA.distance - raycastHitB.distance); }); //Built in system function to easily sort array. Use delegate to sort by distance, has to return int so cast it.
                                   
        
        foreach (RaycastHit raycastHit in raycastHitArray)               //Raycasts all objects in front of mouse pos. Cycle thru them. If have renderer, then we have a hit.
        {
            if (raycastHit.transform.TryGetComponent(out Renderer renderer))
            {
                if (renderer.enabled)
                    return raycastHit.point;
            }

        }
        return Vector3.zero;     //If doesnt hit anything.
    }


}
