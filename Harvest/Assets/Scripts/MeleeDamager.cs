using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DamagerBase : MonoBehaviour
{
    public virtual float GetAttackRange()
    {
        return 0f;
    }
    public virtual float GetAttackDamage()
    {
        return 0f;
    }
    public virtual float GetAttackSpeed()
    {
        return 0f;
    }
    public virtual void Damage(Health health)
    {

    }
}

public class MeleeDamager : MonoBehaviour
{
    [SerializeField, Tooltip("How much damage per hit")]
    private float damage;
    [SerializeField, Tooltip("How many seconds between each attack")]
    private float attackSpeed;
    [SerializeField, Tooltip("How far away it attacker can attack")]
    private float attackRange;

    public float GetAttackRange()
    {
        return attackRange;
    }

    public float GetAttackDamage()
    {
        return damage;
    }

    public float GetAttackSpeed()
    {
        return attackSpeed;
    }

    public void Damage(Health health)
    {
        health.Damage(damage);
    }
}
