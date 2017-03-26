using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damager : MonoBehaviour {

    [SerializeField, Tooltip("The damage for each attack/bullet")]
    protected float damage;
    [SerializeField, Tooltip("At what range this object can attack")]
    protected float attackRange;
    [SerializeField, Tooltip("How often this object should attack")]
    protected float attackSpeed;
    [SerializeField, Tooltip("Define which object should rotate towards the enemy")]
    private GameObject objectToBeRotated;
    [SerializeField, Tooltip("How fast the object should rotate toward the enemy")]
    private float rotationSpeed;

    public float AttackRange { get { return attackRange; } }

    private float timeAtLastAttack;

    /// <summary>
    /// Rotates the object towards the target and checks if it pointing in the right direction.
    /// </summary>
    /// <returns>Whether the object is pointing toward the target</returns>
    protected bool RotateTowardsTarget()
    {
        if (objectToBeRotated != null && GetComponent<TargetFinder>().Target != null)
        {
            float step = rotationSpeed * Time.deltaTime;
            Vector3 targetDir = GetComponent<TargetFinder>().Target.transform.position - objectToBeRotated.transform.position;
            targetDir = new Vector3(targetDir.x, 0f, targetDir.z);
            Vector3 newDir = Vector3.RotateTowards(objectToBeRotated.transform.forward, targetDir, step, 0.0f);
            objectToBeRotated.transform.rotation = Quaternion.LookRotation(newDir);

            if (Vector3.Angle(targetDir, newDir) < 5f)
            {
                return true;
            }
        }
        return false;
    }

    protected bool IsReady()
    {
        if (Time.time - timeAtLastAttack > attackSpeed)
        {
            timeAtLastAttack = Time.time;
            return true;
        }
        return false;
    }

    protected bool IsTargetClose()
    {
        var target = GetComponent<TargetFinder>().Target;
        if (target != null)
        {
            
            if (Vector3.Distance(target.transform.position, this.transform.position) <= attackRange)
            {
                return true;
            }
        }
        return false;
    }
}
