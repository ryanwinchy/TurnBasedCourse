using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{

    [SerializeField] Animator animator;

    //Transform and game object pretty much interchangeable.
    [SerializeField] Transform bulletProjectilePrefab;
    [SerializeField] Transform shootPointTransform;

    private void Awake()
    {
        if (TryGetComponent<MoveAction>(out MoveAction moveAction))     //equivalent to MoveAction moveAction = GetComponent<MoveAction> then if != null. Saying try to get component of moveaction, if found, put in moveAction var, run if statement.
        {                                                                     //Need this check because some units will not have some actions.
            moveAction.OnStartMoving += MoveAction_OnStartMoving;  //Subscribes to event.
            moveAction.OnStopMoving += MoveAction_OnStopMoving;  //Subscribes to event.
        }

        if (TryGetComponent<ShootAction>(out ShootAction shootAction))     
        {                                                                     
            shootAction.OnShoot += ShootAction_OnShoot;  //Subscribes to event.
        }
    }

    void MoveAction_OnStartMoving(object sender, EventArgs e)
    {
        animator.SetBool("Moving", true);
    }

    void MoveAction_OnStopMoving(object sender, EventArgs e)
    {
        animator.SetBool("Moving", false);
    }

    void ShootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        animator.SetTrigger("Shoot");

        Transform bulletProjectileTransform = Instantiate(bulletProjectilePrefab, shootPointTransform.position, Quaternion.identity);
        BulletProjectile newBulletProjectile = bulletProjectileTransform.GetComponent<BulletProjectile>();

        Vector3 targetUnitShootAtPosition = e.targetUnit.transform.position;

        targetUnitShootAtPosition.y = shootPointTransform.position.y;       //So shoots at same level as shoot point, not at feet of target.

        newBulletProjectile.Setup(targetUnitShootAtPosition);
    }



}