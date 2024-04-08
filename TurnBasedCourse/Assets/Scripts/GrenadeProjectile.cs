using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GrenadeProjectile : MonoBehaviour
{

    public static event EventHandler OnAnyGrenadeExploded;        //Static event, belongs to class as a whole not every single prefab with an instance of the script. So is on ANY.

    Vector3 targetPosition;
    [SerializeField] Transform explodeVFXPrefab;
    [SerializeField] TrailRenderer trailRenderer;

    [SerializeField] AnimationCurve arcYAnimationCurve;

    float totalDistance;
    Vector3 positionXZ;

    Action onGrenadeBehaviourComplete;

    private void Update()      //Move towards target pos, destroy itself, make physics sphere and damage all unit colliders within its radius.
    {

        Vector3 moveDirection = (targetPosition - positionXZ).normalized;    //Normalized gives us direction only, magnitude not the actual distance.

        float moveSpeed = 15f;
        positionXZ += moveDirection * moveSpeed * Time.deltaTime;

        float currentDistance = Vector3.Distance(positionXZ, targetPosition);
        float distanceNormalized = 1 - (currentDistance / totalDistance);    //1 minus , otherwise inverted. Should approach one as gets close.

        float maxHeight = totalDistance / 4f;                  //The further the distance the higher we throw it.
        float positionY = arcYAnimationCurve.Evaluate(distanceNormalized) * maxHeight;   //We drew curve we want to apply, like an arc.
        transform.position = new Vector3(positionXZ.x, positionY, positionXZ.z);    //This applies nice animation curve that we set so the grenade arcs when thrown.

        float reachedTargetDistance = 0.2f;
        if (Vector3.Distance(positionXZ, targetPosition) < reachedTargetDistance)
        {
            float damageRadius = 4f;
            Collider[] colliderArray = Physics.OverlapSphere(targetPosition, damageRadius);       //Makes a sphere when grenade gets to target, collects all colliders within sphere.

            foreach (Collider collider in colliderArray)
            {
                if (collider.TryGetComponent<Unit>(out Unit targetUnit))          //same as Unit targetUnit = collider.GettComponent and then an if != null
                    targetUnit.Damage(30);     //Could adapt this to be distance based!
                if (collider.TryGetComponent<DestructibleCrate>(out DestructibleCrate destructibleCrate))          //same as Unit targetUnit = collider.GettComponent and then an if != null
                    destructibleCrate.Damage();     //Could adapt this to be distance based!
            }

            OnAnyGrenadeExploded?.Invoke(this, EventArgs.Empty);     //Fire event on grenade explosion.

            trailRenderer.transform.parent = null;     //Same logic as bullet projectile, otherwise trail bugs when game object destroyed.
            Instantiate(explodeVFXPrefab, targetPosition + Vector3.up * 1f, Quaternion.identity);
            Destroy(gameObject);

            onGrenadeBehaviourComplete();   //Call delegate function that is storing actionComplete() from GrenadeAction script, so cant spam grenades til last one exploded.
        }
    }

    public void Setup(GridPosition targetGridPosition, Action onGrenadeBehaviourComplete)   //We are receiving a delegate function (action so no return or param). Store function. In this case it is so we can call ActionComplete on the grenade itself once it's exploded.
    {
        this.onGrenadeBehaviourComplete = onGrenadeBehaviourComplete;     //Store action complete function from GrenadeAction script in this delegate so can call it from here when grenade exploded.

        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition); //Need world pos.

        positionXZ = transform.position;
        positionXZ.y = 0f;
        totalDistance = Vector3.Distance(positionXZ, targetPosition);
    }
}
