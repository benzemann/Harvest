using UnityEngine;
using System.Collections;

public class WarriorAnt : MonoBehaviour {

    Feromones feromones;
    GameObject target;
    Vector3 targetLastPos;
    Seeker seeker;
    GameObject hive;
    GameObject enemyShield;
    State oldState;
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
    float timeSinceLastMove;
    Vector3 lastPos;

    enum State { IDLE, PATROLING, RETURNHOME, ATTACKING, GOINGTOBEACON, ATTACKINGSHIELD };

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
		if(state == State.ATTACKING || state == State.GOINGTOBEACON){
            if(target == null)
			    transform.GetChild(0).GetComponent<Collider>().isTrigger = false;
            else if(Vector3.Distance(transform.position, target.transform.position) < 3f)
                transform.GetChild(0).GetComponent<Collider>().isTrigger = true;
        } else {
			transform.GetChild(0).GetComponent<Collider>().isTrigger = true;
		}
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
                            if (target.GetComponent<Turret>() != null)
                                target.GetComponent<Turret>().Damage(damagePrSec);
                            else if (target.GetComponent<Harvester>())
                                target.GetComponent<Harvester>().Damage(damagePrSec);
                            else
                                target.GetComponent<Refinery>().Damage(damagePrSec);
                        }
                    } else
                    {
                        if(Vector3.Distance(target.transform.position, targetLastPos) > 1.0f)
                        {
                            seeker.StartPath(transform.position, target.transform.position);
                            targetLastPos = target.transform.position;
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
            case State.ATTACKINGSHIELD:
                if(enemyShield == false || enemyShield.GetComponent<Shield>().IsDisabled())
                {
                    GetComponent<AIPath>().canMove = true;
                    state = oldState;
                } else
                {
                    if (Time.time - timeSinceLastAttack >= 1.0f)
                    {
                        timeSinceLastAttack = Time.time;
                        enemyShield.GetComponent<Shield>().Damage(damagePrSec);
                    }
                }
                break;
        }
        if (state != State.ATTACKING)
        {
            timeSinceLastMove += Time.deltaTime;
            if (Vector3.Distance(transform.position, lastPos) > 0.5f)
            {
                lastPos = transform.position;
                timeSinceLastMove = 0f;
            }
            if (timeSinceLastMove > 30f)
            {
                Debug.Log("Ant not moving! Transportet to home");
                hive.GetComponent<Hive>().EnterWarriorHive();
                Destroy(this.gameObject);
            }
        }
    }

    bool CheckForProtectionTrail()
    {
        feromones.GetCurrentGridCoords(transform.position, out gridX, out gridY);
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

    public void EnterShield(GameObject s)
    {
        enemyShield = s;
        oldState = state;
        state = State.ATTACKINGSHIELD;
        GetComponent<AIPath>().canMove = false;
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
                state = State.GOINGTOBEACON;
                beaconPos = enemy.transform.position;
                return true;
            }
        }
        foreach (GameObject beacon in hive.GetComponent<Hive>().GetDistressBeacons())
        {
            if (Vector3.Distance(transform.position, beacon.transform.position) < beaconFleeingDistance && fleeing == false && state != State.GOINGTOBEACON)
            {
                state = State.GOINGTOBEACON;
                beaconPos = beacon.transform.position;
                return true;
            }
        }
        return false;
    }
    public void Damage(float d)
    {
        if(target != null)
        {
            if(target.name == "Harvester")
            {
                target = null;
                // try to get new target
                GetNewTarget();
            }
        }
        health -= d;
        if (health <= 0f)
        {
            hive.GetComponent<Hive>().KillWarrior();
            Destroy(this.gameObject);
        }

    }

    public string[] InfoText()
    {
        string[] infoText = new string[3];
        infoText[0] = "Warrior ant";
        infoText[1] = "Health: " + health + "/" + maxHealth;
        switch (state)
        {
            case State.IDLE:
                infoText[0] = "State: Idle";
                break;
            case State.GOINGTOBEACON:
                infoText[0] = "State: GointToBeacon";
                break;
            case State.ATTACKING:
                infoText[0] = "State: Attacking";
                break;
            case State.PATROLING:
                infoText[0] = "State: Patroling";
                break;
            case State.RETURNHOME:
                infoText[0] = "State: ReturnHome";
                break;
        }
        return infoText;
    }
}
