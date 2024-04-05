using System;
using UnityEngine;



//Purposefully very generic so can go on anything, players, enemies, walls etc...
public class HealthSystem : MonoBehaviour
{
    public event EventHandler OnDead;
    public event EventHandler OnDamaged;

    [SerializeField] int health = 100;
    int healthMax;

    private void Awake()
    {
        healthMax = health;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health < 0)
            health = 0;

        OnDamaged?.Invoke(this, EventArgs.Empty);     //Fire event.

        if (health == 0)
            Die();
    }

    void Die()
    {
        OnDead?.Invoke(this, EventArgs.Empty);          //OnDead event, more generic. Different things with health systems can die in different ways.
    }

    public float GetHealthNormalized() => (float)health / healthMax;     //Type cast int to float.


}
