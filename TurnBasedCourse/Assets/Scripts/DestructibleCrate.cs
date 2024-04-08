using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using System;

public class DestructibleCrate : MonoBehaviour
{

    public static event EventHandler OnAnyDestroyed;

    GridPosition gridPosition;

    [SerializeField] Transform crateDestroyedPrefab;

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
    }

    public GridPosition GetGridPosition() => gridPosition;
    public void Damage()
    {
        Transform crateDestroyedTransform = Instantiate(crateDestroyedPrefab, transform.position, transform.rotation);

        ApplyExplosionToChildren(crateDestroyedTransform, 150, transform.position, 10);
        Destroy(gameObject);

        OnAnyDestroyed?.Invoke(this, EventArgs.Empty);
    }



    void ApplyExplosionToChildren(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))        //Adds explosion to all direct children in route, so all rigidbodies on ragdoll.
            {
                childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);     //Built in physics function.
            }

            ApplyExplosionToChildren(child, explosionForce, explosionPosition, explosionRange);     //Recursion so applies force to children of children, bones have many layers.

        }
    }

}
