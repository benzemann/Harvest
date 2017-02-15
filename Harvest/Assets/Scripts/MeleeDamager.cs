using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TargetFinder))]
public class MeleeDamager : MonoBehaviour
{
    [SerializeField, Tooltip("How much damage per hit")]
    private float damage;
    [SerializeField, Tooltip("How many seconds between each attack")]
    private float attackSpeed;
    [SerializeField, Tooltip("How far away it attacker can attack")]
    private float attackRange;
    [SerializeField, Tooltip("Define which object should rotate towards the enemy")]
    private GameObject objectToBeRotated;

    public float AttackRange { get { return attackRange; } }

    private float timeAtLastAttack;

    private void Update()
    {
        if(GetComponent<TargetFinder>().Target != null)
        {
            var target = GetComponent<TargetFinder>().Target;
            if(Vector3.Distance(target.transform.position, this.transform.position) <= attackRange)
            {
                if (objectToBeRotated != null)
                {
                    float step = 5f * Time.deltaTime;
                    Vector3 targetDir = target.transform.position - objectToBeRotated.transform.position;
                    targetDir = new Vector3(targetDir.x, objectToBeRotated.transform.position.y, targetDir.z);
                    Vector3 newDir = Vector3.RotateTowards(objectToBeRotated.transform.forward, targetDir, step, 0.0f);
                    objectToBeRotated.transform.rotation = Quaternion.LookRotation(newDir);
                }

                if (Time.time - timeAtLastAttack > attackSpeed)
                {
                    timeAtLastAttack = Time.time;
                    Damage(target.GetComponent<Health>());
                }
            }
        }
    }
    
    private void Damage(Health health)
    {
        health.Damage(damage);
    }
}
