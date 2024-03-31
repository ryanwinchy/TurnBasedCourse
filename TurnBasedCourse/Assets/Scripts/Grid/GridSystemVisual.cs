using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{

    public static GridSystemVisual Instance { get; private set; }       //Singleton so easy for scripts to ref. 

    [SerializeField] Transform gridSystemVisualSinglePrefab;       //Remember, we can access prefab with GameObject or Transform.

    GridSystemVisualSingle[,] gridSystemVisualSingleArray;

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
        gridSystemVisualSingleArray = new GridSystemVisualSingle[LevelGrid.Instance.GetWidth(), LevelGrid.Instance.GetHeight()];  //Setup array of total grid width and height.

        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);

                Transform gridSystemVisualSingleTransform = 
                    Instantiate(gridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);

                gridSystemVisualSingleArray[x, z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();  //Populate array with the singles.
            }
        }
    }

    private void Update()
    {
        UpdateGridVisual();
    }

    public void HideAllGridPositionVisuals()
    {
        for (int x = 0; x < gridSystemVisualSingleArray.GetLength(0); x++)  //Cycle thru 2d array of grid singles.
        {
            for (int z = 0; z < gridSystemVisualSingleArray.GetLength(1); z++)
            {
                gridSystemVisualSingleArray[x, z].Hide();    //Hide all grid system visual singles.
            }
        }
    }

    public void ShowGridPositionList(List<GridPosition> gridPositionList)
    {
        if (gridPositionList == null)
            return;

        foreach (GridPosition gridPosition in gridPositionList)
            gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].Show();
    }

    void UpdateGridVisual()
    {
        HideAllGridPositionVisuals();

        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();

        ShowGridPositionList(selectedAction.GetValidActionGridPositionList());  //For selected action, get valid squares, show them on grid.
    }

}
