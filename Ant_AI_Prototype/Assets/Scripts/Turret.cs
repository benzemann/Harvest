using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour {

    public float range;
    public float rotationSpeed;
    public float damage;
    public float fireCoolDownTime;
    public float health;
    GameObject target;
    public GameObject turretHead;
    public Transform barrelExit;
    public GameObject bullet;
    float timeSinceLastShot;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if(target != null)
            if (Vector3.Distance(target.transform.position, transform.position) > range)
                target = null;
        if (target == null)
            FindNewTarget();
        else
        {
            Vector3 dir = target.transform.position - turretHead.transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Vector3 rotation = Quaternion.Lerp(turretHead.transform.rotation, lookRotation, rotationSpeed * Time.deltaTime).eulerAngles;
            turretHead.transform.localRotation = Quaternion.Euler(0f,rotation.y,0f);

            if(Time.time - timeSinceLastShot > fireCoolDownTime)
            {
                Shoot();
                timeSinceLastShot = Time.time;
            }
        }
	}

    void Shoot()
    {
        GameObject bulletGO = Instantiate(bullet, barrelExit.position, barrelExit.rotation) as GameObject;
        Bullet b = bulletGO.GetComponent<Bullet>();
        if(b != null)
        {
            b.Seek(target,damage);
        }
    }

    void FindNewTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Ants");
        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;
        foreach(GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(enemy.transform.position, transform.position);
            if(distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }
        if(closestEnemy != null)
        {
            if(closestDistance <= range)
            {
                target = closestEnemy;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    public void Damage(float d)
    {
        health -= d;
        if (health <= 0)
            Destroy(this.gameObject);
    }
}
