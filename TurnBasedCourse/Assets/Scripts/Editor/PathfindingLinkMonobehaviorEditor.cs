using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathfindingLinkMonobehavior))]
public class PathfindingLinkMonobehaviorEditor : Editor
{

    private void OnSceneGUI()
    {
        PathfindingLinkMonobehavior pathfindingLinkMonobehavior = (PathfindingLinkMonobehavior)target;


        EditorGUI.BeginChangeCheck();

        Vector3 newLinkPositionA = Handles.PositionHandle(pathfindingLinkMonobehavior.linkPositionA, Quaternion.identity);
        Vector3 newLinkPositionB = Handles.PositionHandle(pathfindingLinkMonobehavior.linkPositionB, Quaternion.identity);


        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(pathfindingLinkMonobehavior, "Change Link Position");
            pathfindingLinkMonobehavior.linkPositionA = newLinkPositionA;
            pathfindingLinkMonobehavior.linkPositionB = newLinkPositionB;

        }


    }
}
