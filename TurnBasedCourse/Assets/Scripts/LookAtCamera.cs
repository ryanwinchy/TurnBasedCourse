using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{

    [SerializeField] bool invert;
    Transform cameraTransform;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }

    private void LateUpdate()         //Late update always runs after all the other updates. So runs after camera moves for eg.
    {
        if (invert)
        {
            Vector3 directionToCamera = (cameraTransform.position - transform.position).normalized;     //Normalized as its a direction, need the direction not distance, keeps magnitude only.
            transform.LookAt(transform.position + directionToCamera * -1f);     
        }
        else
            transform.LookAt(cameraTransform);
    }
}
