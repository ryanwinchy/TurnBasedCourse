using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridDebugObject : MonoBehaviour
{
    GridObject gridObject;

    [SerializeField] TextMeshPro gridDebugText;


    public void SetGridObject(GridObject gridObject)
    {
        this.gridObject = gridObject;
    }

    private void Update()
    {
        gridDebugText.text = gridObject.ToString();
    }
}
