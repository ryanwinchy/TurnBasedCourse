using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    GridSystem gridSystem;

    [SerializeField] Transform gridDebugObjectPrefab;

    private void Start()
    {
        gridSystem = new GridSystem(10, 10, 2f);
        gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
    }

    private void Update()
    {
        
    }

}
