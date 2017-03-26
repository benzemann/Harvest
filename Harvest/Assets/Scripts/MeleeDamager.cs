using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TargetFinder))]
public class MeleeDamager : Damager
{
    
    private void Update()
    {
        if (IsTargetClose())
        {
            RotateTowardsTarget();
                
            if (IsReady())
            {
                var target = GetComponent<TargetFinder>().Target;
                Damage(target.GetComponent<Health>());
            }

        }
    }
    
    private void Damage(Health health)
    {
        health.Damage(damage);
    }
}
