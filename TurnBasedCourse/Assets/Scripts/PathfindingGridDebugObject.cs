using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


//More specific debug object with the components needed for a*.
public class PathfindingGridDebugObject : GridDebugObject
{ 
    [SerializeField] TextMeshPro gCostText;       //a* components, see my notes. Makes perfect sense.
    [SerializeField] TextMeshPro hCostText;
    [SerializeField] TextMeshPro fCostText;

    PathNode pathNode;       //node in the pathfinding grid, like a square.
    public override void SetGridObject(object gridObject)
    {
        
        base.SetGridObject(gridObject);

        pathNode = (PathNode)gridObject;          //Cast the generic gridObject to a path node.

    }

    protected override void Update()
    {
        base.Update();       //Updates base text (the normal grid pos).

        gCostText.text = pathNode.GetGCost().ToString();
        hCostText.text = pathNode.GetHCost().ToString();
        fCostText.text = pathNode.GetFCost().ToString();
    }



}
