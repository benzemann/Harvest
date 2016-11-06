﻿using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour {

    public float range;
    public float rotationSpeed;
    public float damage;
    public float fireCoolDownTime;
    public float maxHealth;
    public float repairTime;
    public float repairAmount;
    public float repairCosts;
    public float costs;
    public string textOnButton;
    public float buildTime;
    public float scaffoldUpTime;
    public float scaffoldDownTime;
    public float scaffoldStartHeight;
    public float scaffoldEndHeight;
    public float turretStartHeight;
    public float turretEndHeight;
    public enum TurretType { Phasma, Fire, Missile, Shield };
    public TurretType turretType;
    public float health;
    GameObject target;
    public GameObject turretHead;
    public Transform barrelExit;
    public GameObject bullet;
    public GameObject fireCollider;
    public GameObject scaffold;
    public GameObject repairPrefab;
    public Vector3 repairLocalPos;
    GameObject repairGO;
    GameObject scaffoldGO;
    bool shieldHasBeenInit = false;

    Vector3 startPos;
    bool isReady = false;
    float timeSinceLastShot;
    float timeSinceLastRepair;
    float timeBuilding;

	// Use this for initialization
	void Start () {
        health = maxHealth;
        GameObject.Find("A*").GetComponent<AstarPath>().Scan();
        startPos = new Vector3(transform.position.x, turretStartHeight, transform.position.z);
        transform.position = startPos;
        scaffoldGO = Instantiate(scaffold, new Vector3( transform.position.x, scaffoldStartHeight, transform.position.z ), Quaternion.identity) as GameObject;
    }
	
	// Update is called once per frame
	void Update () {
        if (isReady)
        {
            if(turretType != TurretType.Shield)
            {
                if (target != null)
                {
                    if (Vector3.Distance(target.transform.position, transform.position) > range)
                        target = null;
                    if (!CanISeeIt(target))
                        target = null;
                }

                if (target == null)
                    FindNewTarget();
                else
                {
                    Vector3 dir = target.transform.position - turretHead.transform.position;
                    Quaternion lookRotation = Quaternion.LookRotation(dir);
                    Vector3 rotation = Quaternion.Lerp(turretHead.transform.rotation, lookRotation, rotationSpeed * Time.deltaTime).eulerAngles;
                    turretHead.transform.localRotation = Quaternion.Euler(0f, rotation.y, 0f);

                    if (Time.time - timeSinceLastShot > fireCoolDownTime)
                    {
                        Shoot();
                        timeSinceLastShot = Time.time;
                    }
                }
            } else
            {
                if (!shieldHasBeenInit)
                {
                    GetComponentInChildren<Shield>().Enable();
                    shieldHasBeenInit = true;
                }
            }
            if (health < maxHealth)
            {
                timeSinceLastRepair += Time.deltaTime;
                if (timeSinceLastRepair >= repairTime)
                {
                    if (GameObject.Find("Player").GetComponent<Player>().Pay(repairCosts))
                    {
                        if (repairGO == null)
                            InstatiateRepairGO();
                        health += repairAmount;
                        if (health > maxHealth)
                        {
                            health = maxHealth;
                        }
                        timeSinceLastRepair = 0f;
                    } else
                    {
                        RemoveRepairGO();
                    }
                }
            }
            else
            {
                RemoveRepairGO();
            }
        } else
        {
            timeBuilding += Time.deltaTime;
            if(timeBuilding <= scaffoldUpTime)
            {
                transform.position = new Vector3(transform.position.x, turretStartHeight, transform.position.z);
                scaffoldGO.transform.position = new Vector3(transform.position.x, (1.0f-(timeBuilding / scaffoldUpTime)) * scaffoldStartHeight + (timeBuilding/scaffoldUpTime) * scaffoldEndHeight, transform.position.z);
            }
            if(timeBuilding > scaffoldUpTime && timeBuilding <= (scaffoldUpTime + buildTime))
            {
                float percentage = (timeBuilding - scaffoldUpTime) / (buildTime);
                transform.position = new Vector3(transform.position.x, (1.0f - percentage) * turretStartHeight + percentage * turretEndHeight, transform.position.z);
            }
            if(timeBuilding <= (scaffoldUpTime + buildTime + scaffoldDownTime) && timeBuilding > scaffoldUpTime + buildTime)
            {
                float percentage = (timeBuilding - (scaffoldUpTime + buildTime)) / (scaffoldDownTime);
                scaffoldGO.transform.position = new Vector3(transform.position.x, (1.0f - percentage) * scaffoldEndHeight + percentage * scaffoldStartHeight, transform.position.z);
            }
            if(timeBuilding >= buildTime + scaffoldUpTime + scaffoldDownTime)
            {
                Destroy(scaffoldGO);
                GameObject.Find("A*").GetComponent<AstarPath>().Scan();
                isReady = true;
            }
        }
	}

    void InstatiateRepairGO()
    {
        if (repairPrefab != null && repairGO == null)
        {
            repairGO = Instantiate(repairPrefab, repairLocalPos, Quaternion.identity) as GameObject;
            repairGO.transform.parent = this.transform;
            repairGO.transform.localPosition = repairLocalPos;
        }
    }

    void RemoveRepairGO()
    {
        if (repairGO != null)
            Destroy(repairGO);
    }

    void Shoot()
    {
        GameObject bulletGO = Instantiate(bullet, barrelExit.position, barrelExit.rotation) as GameObject;
        Bullet b = bulletGO.GetComponent<Bullet>();
        if(b != null)
        {
            if (turretType == TurretType.Phasma || turretType == TurretType.Missile)
                b.Seek(target, damage);
            else
            {
				b.Seek(fireCollider.transform.GetChild(0).gameObject, 0.0f);
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Ants");
                foreach(GameObject enemy in enemies)
                {
                    if (fireCollider.GetComponent<Collider>().bounds.Contains(enemy.transform.position))
                    {
                        if(enemy.GetComponent<Ant>() != null)
                        {
                            enemy.GetComponent<Ant>().Damage(damage);
                        } else if (enemy.GetComponent<WarriorAnt>() != null)
                        {
                            enemy.GetComponent<WarriorAnt>().Damage(damage);
                        }
                    }
                }
                
            }
                
        }
    }
    
    bool CanISeeIt(GameObject t)
    {
        RaycastHit hit;
        if (t == null)
            return false;
        LayerMask layerMask = new LayerMask();
        layerMask = 1 << LayerMask.NameToLayer("Ants") | 1 << LayerMask.NameToLayer("Obstacles");
        if (Physics.Raycast(barrelExit.transform.position, t.transform.position - barrelExit.position , out hit, 1000f, layerMask))
        {
            if (hit.transform.gameObject.tag == "Ants")
                return true;
        }
        return false;
    }

    void FindNewTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Ants");
        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;
        foreach(GameObject enemy in enemies)
        {
			if(enemy.GetComponent<Ant>() == null && enemy.GetComponent<WarriorAnt>() == null)
				continue;
            float distance = Vector3.Distance(enemy.transform.position, transform.position);
            if(distance < closestDistance)
            {
                if (CanISeeIt(enemy))
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
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
        {
            if (scaffoldGO != null)
                Destroy(scaffoldGO);
            Destroy(this.gameObject);
        }
            
    }

    public string[] InfoText()
    {
        string[] infoText = new string[2];
        infoText[0] = "Turret";
        infoText[1] = "Health : " + health + "/" + maxHealth;
        return infoText;
    }

    public void Repair(float amount)
    {
        health += amount;
        if (health > maxHealth)
            health = maxHealth;
    }
}
