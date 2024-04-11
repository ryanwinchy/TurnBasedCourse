using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorVisibility : MonoBehaviour
{

    Renderer[] rendererArray;
    int floor;

    [SerializeField] bool dynamicFloorPosition;

    [SerializeField] List<Renderer> ignoreRendererList;
    

    private void Awake()
    {
        rendererArray = GetComponentsInChildren<Renderer>(true);       //This returns array of all renderers in children. Renderer is base for cube renderer, mesh etc... End param is to include inactive.

    }

    private void Start()
    {
        floor = LevelGrid.Instance.GetFloor(transform.position);

        if (floor == 0 && !dynamicFloorPosition)                 //Objects on bottom floor dont need this script, destroy for optimization.
            Destroy(this);
    }

    private void Update()
    {
        float cameraHeight = CameraController.Instance.GetCameraHeight();

        float floorHeightOffset = 2f;       //So hidden floor becomes visible a big higher.

        if (dynamicFloorPosition)            //This is a tiny bit wasteful, could optimize and make event instead of update every second. However, only on units and aren't that many, so isn't a problem.
        {
            floor = LevelGrid.Instance.GetFloor(transform.position);
        }


        bool showObject = cameraHeight > LevelGrid.FLOOR_HEIGHT * floor + floorHeightOffset;

        if (showObject || floor == 0)
            Show();
        else
            Hide();
    }

    void Show()
    {
        foreach (Renderer renderer in rendererArray)
        {
            if (ignoreRendererList.Contains(renderer))
                continue;

            renderer.enabled = true;
        }
    }

    void Hide()
    {
        foreach (Renderer renderer in rendererArray)
        {
            if (ignoreRendererList.Contains(renderer))
                continue;

            renderer.enabled = false;
        }
    }
}
