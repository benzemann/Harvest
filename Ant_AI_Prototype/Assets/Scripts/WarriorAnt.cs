using UnityEngine;
using System.Collections;

public class WarriorAnt : MonoBehaviour {

    Feromones feromones;
    GameObject target;
    Seeker seeker;
    GameObject hive;

    public float defendFeromoneAmount;
    public float distressDistance;
    public float beaconFleeingDistance;
    public float attackDistance;
    public float damagePrSec;
    public float maxHealth;
    float health;
    bool fleeing = false;
    bool hasCalculatedPathToBeacon = false;
    Vector3 beaconPos;
    int lastX;
    int lastY;
    int gridX;
    int gridY;
    bool goingOut = true;
    float timeSinceLastAttack;

    enum State { IDLE, PATROLING, RETURNHOME, ATTACKING, GOINGTOBEACON };

    State state;

    // Use this for initialization
    void Start () {
        init();
    }

    public void init()
    {
        if (state == State.GOINGTOBEACON)
            return;
        feromones = GameObject.Find("FeromoneHandler").GetComponent<Feromones>();
        seeker = GetComponent<Seeker>();
        feromones.GetCurrentGridCoords(transform.position, out gridX, out gridY);
        health = maxHealth;
    }
	
	// Update is called once per frame
	void Update () {
        switch (state)
        {
            case State.IDLE:
                if (LookForEnemies())
                {
                    break;
                }
                if (CheckForProtectionTrail())
                {
                    state = State.PATROLING;
                } else
                {
                    state = State.RETURNHOME;
                    seeker.StartPath(transform.position, hive.transform.position);
                }
                break;
            case State.PATROLING:
                if (LookForEnemies())
                {
                    break;
                }
                break;
            case State.RETURNHOME:

                if (Vector3.Distance(transform.position, hive.transform.position) < 0.85f)
                {
                    hive.GetComponent<Hive>().EnterWarriorHive();
                    Destroy(this.gameObject);
                }
                break;
            case State.ATTACKING:
                if(target == null)
                {
                    if (!GetNewTarget())
                    {
                        state = State.RETURNHOME;
                        seeker.StartPath(transform.position, hive.transform.position);
                    }
                } else
                {
                    if(Vector3.Distance(transform.position, target.transform.position) < attackDistance)
                    {
                        if(Time.time - timeSinceLastAttack >= 1.0f)
                        {
                            timeSinceLastAttack = Time.time;
                            target.GetComponent<Turret>().Damage(damagePrSec);
                        }
                        
                    }
                }
                break;
            case State.GOINGTOBEACON:
                if (!hasCalculatedPathToBeacon)
                    AttackBeacon(beaconPos);
                if (GetNewTarget())
                {
                    state = State.ATTACKING;
                }
                break;
        }
	}

    bool CheckForProtectionTrail()
    {
        Vector3 protectionTrail = feromones.GetProtectionOfFeromoneTrail(gridX, gridY, goingOut);
        //Debug.Log(protectionTrail + " " + gridX + " " + gridY);
        if(protectionTrail.x != -1000.0f)
        {
            seeker.StartPath(transform.position, protectionTrail);
            return true;
        }
        return false;
    }

    public void PathComplete()
    {
        feromones.GetCurrentGridCoords(transform.position, out gridX, out gridY);
        if (state == State.PATROLING)
        {
            AddDefendTrail();
            if (!CheckForProtectionTrail())
            {
                goingOut = !goingOut;
                if(goingOut == true)
                {
                    state = State.RETURNHOME;
                    seeker.StartPath(transform.position, hive.transform.position);
                    AddDefendTrail();
                    return;
                }
                AddDefendTrail();
                if (!CheckForProtectionTrail())
                {
                    state = State.IDLE;
                } 
            }
        }
        if(state == State.GOINGTOBEACON)
        {
            if (!GetNewTarget())
            {
                state = State.RETURNHOME;
                seeker.StartPath(transform.position, hive.transform.position);
            }
        }
    }

    public void NextWaypoint()
    {
        lastX = gridX;
        lastY = gridY;

        feromones.GetCurrentGridCoords(transform.position, out gridX, out gridY);

        if(state == State.PATROLING)
        {
            CheckForProtectionTrail();
            AddDefendTrail();
        }

    }

    void AddDefendTrail()
    {
        if(!goingOut)
            feromones.AddDefendTrail(gridX, gridY, defendFeromoneAmount);
    }

    public void SetHive(GameObject h)
    {
        hive = h;
    }

    public void SetStateToGoingToBeacon(Vector3 bPos)
    {
        state = State.GOINGTOBEACON;
        beaconPos = bPos;
    }

    public void AttackBeacon(Vector3 pos)
    {
        hasCalculatedPathToBeacon = true;

        Vector3 ranVector = new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f));
        if (ranVector.magnitude == 0.0f)
            ranVector = Vector3.right;
        ranVector.Normalize();
        ranVector *= Random.Range(0.1f, 2.0f);

        Vector3 ranPos = pos + ranVector;

        RaycastHit hit;

        if (Physics.Raycast(ranPos + new Vector3(0.0f, 10.0f, 0.0f), Vector3.down, out hit))
        {
            if (hit.transform.gameObject.name != "Ground")
            {
                ranPos = pos;
            } 
        }

        seeker.StartPath(transform.position, ranPos);
    }

    bool GetNewTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Player");
        GameObject closestEnemy = null;
        float closestDistance = 1000000.0f;
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }
        if(closestEnemy != null)
        {
            if(Vector3.Distance(transform.position, closestEnemy.transform.position) < distressDistance)
            {
                target = closestEnemy;
                seeker.StartPath(transform.position, target.transform.position);
                return true;
            }
        }
        return false;
    }

    bool LookForEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject enemy in enemies)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) < distressDistance && fleeing == false && state != State.GOINGTOBEACON)
            {
                hive.GetComponent<Hive>().PutDownDistressBeacon(enemy.transform.position);
                fleeing = true;
                state = State.RETURNHOME;
                seeker.StartPath(transform.position, hive.transform.position);
                return true;
            }
        }
        foreach (GameObject beacon in hive.GetComponent<Hive>().GetDistressBeacons())
        {
            if (Vector3.Distance(transform.position, beacon.transform.position) < beaconFleeingDistance && fleeing == false && state != State.GOINGTOBEACON)
            {
                fleeing = true;
                state = State.RETURNHOME;
                seeker.StartPath(transform.position, hive.transform.position);
                return true;
            }
        }
        return false;
    }
    public void Damage(float d)
    {
        health -= d;
        if (health <= 0f)
            Destroy(this.gameObject);
    }
}
