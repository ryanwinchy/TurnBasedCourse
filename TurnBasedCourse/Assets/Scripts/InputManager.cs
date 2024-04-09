#define USE_NEW_INPUT_SYSTEM           //PReprocessor directive so we can have both input systems at same time, and tell processor which one to use. Runs #if logic at compile time, not during runtime.
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    public static InputManager Instance { get; private set; }          //Singleton. Any class can read, only this class can set. Properties are Pascal case.

    PlayerInputActions playerInputActions;       //Our saved loadout for new input system.

    private void Awake()
    {
        if (Instance != null)          //Singleton. If duplicates, destroys the dupe. So one single instance.
        {
            Destroy(gameObject);
            return;
        }
        else
            Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();    //Enables our input system which we named 'Player'.
    }


    public Vector2 GetMouseScreenPosition()
    {
#if USE_NEW_INPUT_SYSTEM
        return Mouse.current.position.ReadValue();
#else
        return Input.mousePosition;
#endif
    }

    public bool IsMouseButtonDownThisFrame()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.Click.WasPressedThisFrame();
#else
        Input.GetMouseButtonDown(0);
#endif

    }

    public Vector2 GetCameraMoveVector()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraMovement.ReadValue<Vector2>();
#else
        Vector2 inputMoveDir = new Vector3(0, 0);

        if (Input.GetKey(KeyCode.W))
            inputMoveDir.y = +1f;
        if (Input.GetKey(KeyCode.S))
            inputMoveDir.y = +-1f;
        if (Input.GetKey(KeyCode.D))
            inputMoveDir.x = +1f;
        if (Input.GetKey(KeyCode.A))
            inputMoveDir.x = -1f;

        return inputMoveDir;
#endif
    }


    public float GetCameraRotateAmount()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraRotate.ReadValue<float>();
#else
        float rotateAmount = 0f;

        if (Input.GetKey(KeyCode.Q))
        {
            rotateAmount = +1f;
        }
        if (Input.GetKey(KeyCode.E))
        {
            rotateAmount += -1f;
        }


        return rotateAmount;
#endif
    }

    public float GetCameraZoomAmount()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraZoom.ReadValue<float>();

#else

        float zoomAmount = 0f;

        if (Input.mouseScrollDelta.y > 0)        //If scrolling out.
        {
            zoomAmount = -1f;
        }
        if (Input.mouseScrollDelta.y < 0)        //If scrolling in.
        {
            zoomAmount = 1f;
        }

        return zoomAmount;
#endif
    }


}
