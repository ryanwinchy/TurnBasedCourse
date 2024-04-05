using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [SerializeField] TrailRenderer trailRenderer;
    [SerializeField] Transform bulletHitVfxPrefab;

    Vector3 targetPosition;
    public void Setup(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    private void Update()
    {
        Vector3 moveDirection = (targetPosition - transform.position).normalized;   //Normalized means we take the direction (magnitude) not distance.

        float distanceBeforeMoving = Vector3.Distance(transform.position, targetPosition);

        float moveSpeed = 200f;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        float distanceAfterMoving = Vector3.Distance(transform.position, targetPosition);


        if (distanceBeforeMoving < distanceAfterMoving)     //This is super high speed, so have to use this method, less buggy. If dist before is smaller than dist after, destroy bullet.
        {
            transform.position = targetPosition;     //Looks better.

            trailRenderer.transform.parent = null;     //This removes the parent relationship of the trail renderer, so it is not destroyed instantly. Ensure 'auto destruct' is on trail, or will never destroy.
            Destroy(gameObject);

            Instantiate(bulletHitVfxPrefab, targetPosition, Quaternion.identity);   //Vfx explosion on hit.
        }

    }

}
