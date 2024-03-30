using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera cinemachineVirtualCamera;
    CinemachineTransposer cinemachineTransposer;
    Vector3 targetFollowOffset;

    const float MIN_FOLLOW_Y_OFFSET = 2f;    //Constants for zoom limits.
    const float MAX_FOLLOW_Y_OFFSET = 12f;

    private void Start()
    {
        cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();    //Get access to cinemachine script that controls offset for zoom effect.
        targetFollowOffset = cinemachineTransposer.m_FollowOffset;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
    }

    private void HandleMovement()
    {
        Vector3 inputMoveDir = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W))
            inputMoveDir.z = +1f;
        if (Input.GetKey(KeyCode.S))
            inputMoveDir.z = +-1f;
        if (Input.GetKey(KeyCode.D))
            inputMoveDir.x = +1f;
        if (Input.GetKey(KeyCode.A))
            inputMoveDir.x = -1f;




        float moveSpeed = 10f;

        Vector3 moveVector = (transform.forward * inputMoveDir.z) + (transform.right * inputMoveDir.x);      //So moves based on where camera actually facing.


        transform.position += moveVector * moveSpeed * Time.deltaTime;  //Frame rate independent.
    }

    private void HandleRotation()
    {
        Vector3 rotationVector = new Vector3(0, 0, 0);

        //ROTATE CAM
        if (Input.GetKey(KeyCode.Q))
            rotationVector.y = -1f;
        if (Input.GetKey(KeyCode.E))
            rotationVector.y = 1f;

        float rotationSpeed = 100f;

        transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;         //Euler angles simply refers to rotation as seen in editor, like 180 degrees.
    }

    private void HandleZoom()
    {
        float zoomAmount = 1f;     //No need for smoothing and time.delta time as mouseScroll does one unit at a time anyway.
        float zoomSpeed = 5f;


        if (Input.mouseScrollDelta.y > 0)        //If scrolling out.
        {
            targetFollowOffset.y -= zoomAmount;
        }
        if (Input.mouseScrollDelta.y < 0)        //If scrolling in.
        {
            targetFollowOffset.y += zoomAmount;
        }

        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);     //Keeps follow offset between our two zoom limits.


        cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, Time.deltaTime * zoomSpeed);   //lerp smooths between two points.
    }
}
