using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }
    
    [SerializeField] CinemachineVirtualCamera cinemachineVirtualCamera;
    CinemachineTransposer cinemachineTransposer;
    Vector3 targetFollowOffset;

    const float MIN_FOLLOW_Y_OFFSET = 2f;    //Constants for zoom limits.
    const float MAX_FOLLOW_Y_OFFSET = 15f;

    private void Awake()
    {
        if (Instance != null)          //Singleton. If duplicates, destroys the dupe. So one single instance.
        {
            Destroy(gameObject);
            return;
        }
        else
            Instance = this;
    }

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
        Vector2 inputMoveDir = InputManager.Instance.GetCameraMoveVector();

        float moveSpeed = 10f;

        Vector3 moveVector = (transform.forward * inputMoveDir.y) + (transform.right * inputMoveDir.x);      //So moves based on where camera actually facing.


        transform.position += moveVector * moveSpeed * Time.deltaTime;  //Frame rate independent.
    }

    private void HandleRotation()
    {
        Vector3 rotationVector = new Vector3(0, 0, 0);

        rotationVector.y = InputManager.Instance.GetCameraRotateAmount();

        float rotationSpeed = 100f;

        transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;         //Euler angles simply refers to rotation as seen in editor, like 180 degrees.
    }

    private void HandleZoom()
    {
        float zoomIncreaseAmount = 1f;     //No need for smoothing and time.delta time as mouseScroll does one unit at a time anyway.
        float zoomSpeed = 5f;

        targetFollowOffset.y += InputManager.Instance.GetCameraZoomAmount() * zoomIncreaseAmount;

        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);     //Keeps follow offset between our two zoom limits.


        cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, Time.deltaTime * zoomSpeed);   //lerp smooths between two points.
    }




    public float GetCameraHeight() => targetFollowOffset.y;



}
 