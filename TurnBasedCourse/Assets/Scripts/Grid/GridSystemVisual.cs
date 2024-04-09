using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{

    public static GridSystemVisual Instance { get; private set; }       //Singleton so easy for scripts to ref. 



    [Serializable]     //Custom struct, so add this so shows in editor.
    public struct GridVisualTypeMaterial                                //FOR DIFF COLOUR GRID TILES.
    {
        public GridVisualType gridVisualType;
        public Material material;
    }

    public enum GridVisualType { White, Blue, Red, Yellow, RedSoft }

    [SerializeField] List<GridVisualTypeMaterial> gridVisualTypeMaterialList;





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

        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;     //Event subs.
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;

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

    void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();

        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))      //if invalid (is negative or outside grid bounds), moves on to next.
                    continue;

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);       //Mathf as want the positive only, not the negative.
                if (testDistance > range)        //This is creating a triangle of range instead of a square. Saying add up x and y ( like 5 to right 5 up), will be out of range as max is 7.
                    continue;

                gridPositionList.Add(testGridPosition);   //if valid, and within TRIANGLE range, add to list.
            }
        }

        ShowGridPositionList(gridPositionList, gridVisualType);
    }

    void ShowGridPositionRangeSquare(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();

        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))      //if invalid (is negative or outside grid bounds), moves on to next.
                    continue;

                gridPositionList.Add(testGridPosition);   //if valid, and within TRIANGLE range, add to list.
            }
        }

        ShowGridPositionList(gridPositionList, gridVisualType);
    }



    public void ShowGridPositionList(List<GridPosition> gridPositionList, GridVisualType gridVisualType)
    {
        if (gridPositionList == null)
            return;

        foreach (GridPosition gridPosition in gridPositionList)
            gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].Show(GetGridVisualTypeMaterial(gridVisualType));    //Choose which colour to show.
    }

    void UpdateGridVisual()
    {
        HideAllGridPositionVisuals();

        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

        GridVisualType gridVisualType;

        switch (selectedAction)          //Diff colour grid for diff actions.
        {
            default:                                      //Defaults to white.
            case MoveAction moveAction:
                gridVisualType = GridVisualType.White;
                break;
            case SpinAction spinAction:
                gridVisualType = GridVisualType.Blue;
                break;
            case ShootAction shootAction:
                gridVisualType = GridVisualType.Red;

                ShowGridPositionRange(selectedUnit.GetGridPosition(), shootAction.GetMaxShootDistance(), GridVisualType.RedSoft);
                break;
            case GrenadeAction grenadeAction:
                gridVisualType = GridVisualType.Yellow;
                break;
            case SwordAction swordAction:
                gridVisualType = GridVisualType.Red;

                ShowGridPositionRangeSquare(selectedUnit.GetGridPosition(), swordAction.GetMaxSwordDistance(), GridVisualType.RedSoft);
                break;
            case InteractAction interactAction:
                gridVisualType = GridVisualType.Blue;
                break;


        }



        ShowGridPositionList(selectedAction.GetValidActionGridPositionList(), gridVisualType);  //For selected action, get valid squares, show them on grid.
    }

    void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    void LevelGrid_OnAnyUnitMovedGridPosition(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)   //Give tile colour.
    {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList)  //Cycle thru material list.
        {
            if (gridVisualTypeMaterial.gridVisualType == gridVisualType)  //if colour matches.
                return gridVisualTypeMaterial.material;          //Return material.
        }
        Debug.LogError("Could not find grid visual type material for GridVisualType" + gridVisualType);
        return null;
    }

}
