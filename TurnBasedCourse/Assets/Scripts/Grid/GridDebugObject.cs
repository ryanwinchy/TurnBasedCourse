using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridDebugObject : MonoBehaviour
{
    object gridObject;      //Object is default class in c#, can cast anything to object.

    [SerializeField] TextMeshPro gridDebugText;


    public virtual void SetGridObject(object gridObject)    
    {
        this.gridObject = gridObject;
    }

    protected virtual void Update()
    {
        gridDebugText.text = gridObject.ToString();
    }
}
