using UnityEngine;
using System.Collections;

public class Harvester : MonoBehaviour {

    public float maxHealth;
    public float repairPrSec;
    [Tooltip("The time to harvest 1 ressource")]
    public float harvestTime;
    [Tooltip("The time to unload 1 ressource")]
    public float unloadTime;
    public float repairCosts;
    public float ressourceCapacity;
    public float ressources;
    public float ressourceSearchArea;
    public bool goHomeIfNoRes;
    public float health;
    public GameObject repairPrefab;
    public Vector3 repairLocalPos;
    GameObject repairGO;
    float timeSinceLastHarvest;
    float timeSinceLastUnload;
    float timeSinceLastRepair;
    Vector3 lastRessourcePos;

    enum State { Driving, GoToRessource, Harvest, ReturnHome, PlacingPlate, ReturnToRessource };

    State state;
    GameObject target;
    GameObject refinery;
    Seeker seeker;

	// Use this for initialization
	void Start () {
        seeker = GetComponent<Seeker>();
        health = maxHealth;
        state = State.Driving;
        refinery = GameObject.Find("Refinery");
    }
	
	// Update is called once per frame
	void Update () {
	    if(state == State.GoToRessource)
        {
            if(target == null)
            {
                state = State.Driving;
                return;
            }
            if(Vector3.Distance(transform.position, target.transform.position) < 1.5f)
            {
                state = State.Harvest;
                timeSinceLastHarvest = Time.time;
            }
        }
        if(state == State.Harvest)
        {
            if(target == null)
            {
                GameObject closeRes = LookForRessources();
                if (closeRes != null)
                {
                    seeker.StartPath(transform.position, closeRes.transform.position);
                    target = closeRes;
                    state = State.GoToRessource;
                }
                else if (goHomeIfNoRes)
                {
                    GoHome();
                    lastRessourcePos = transform.position;
                }
                else
                {
                    state = State.Driving;
                }
                return;
            }
            if(Time.time - timeSinceLastHarvest > harvestTime)
            {
                if(Vector3.Distance(transform.position, target.transform.position) > 1.5f)
                {
                    state = State.Driving;
                    return;
                }
                timeSinceLastHarvest = Time.time;
                if(ressources < ressourceCapacity)
                {
                    if (target.transform.GetChild(0).gameObject.GetComponent<Ressource>().Harvest(1)) {
                        ressources += 1;
                        if (ressources >= ressourceCapacity)
                        {
                            if (refinery == null)
                                return;
                            GoHome();
                            lastRessourcePos = transform.position;
                        }
                    } else
                    {
                        GameObject closeRes = LookForRessources();
                        if(closeRes != null)
                        {
                            seeker.StartPath(transform.position, closeRes.transform.position);
                            state = State.GoToRessource;
                            target = closeRes;
                        } else if(goHomeIfNoRes)
                        {
                            GoHome();
                            lastRessourcePos = transform.position;
                        }
                    }
                }
            }
        }
        if(state == State.ReturnToRessource)
        {
            if(Vector3.Distance(transform.position, lastRessourcePos) < 1.0f)
            {
                GameObject closeRes = LookForRessources();
                if (closeRes != null)
                {
                    seeker.StartPath(transform.position, closeRes.transform.position);
                    target = closeRes;
                    state = State.GoToRessource;
                }
            }   
        }
        if(state == State.ReturnHome)
        {
            if (refinery != null)
            {
                if (Vector3.Distance(transform.position, refinery.transform.GetChild(0).transform.position) < 1.5f)
                {
                    if (Time.time - timeSinceLastUnload > unloadTime)
                    {
                        if (ressources <= 0)
                        {
                            state = State.Driving;
                            if(lastRessourcePos != null)
                            {
                                state = State.ReturnToRessource;
                                seeker.StartPath(transform.position, lastRessourcePos);
                            }
                            return;
                        }
                        ressources -= 1;
                        GameObject player = GameObject.Find("Player");
                        player.GetComponent<Player>().AddRessources(1);
                        timeSinceLastUnload = Time.time;
                    }
                }
            }
        }
        if(state == State.PlacingPlate)
        {
            if(target == null)
            {
                state = State.Driving;
                return;
            }
            //Debug.Log(Vector3.Distance(transform.position, target.transform.position));
            if(Vector3.Distance(transform.position, target.transform.position) < 2.0f)
            {
                if (target.GetComponent<TurretSpot>() == null)
                    return;
                GameObject player = GameObject.Find("Player");
                if (player.GetComponent<Player>().PayForPlate())
                {
                    target.GetComponent<TurretSpot>().AddPlate();
                    state = State.Driving;
                }
            }
        }
        if (refinery != null)
        {
            if (Vector3.Distance(transform.position, refinery.transform.GetChild(0).transform.position) < 1.5f)
            {
                if (Time.time - timeSinceLastRepair > 1f)
                {
                    if (GameObject.Find("Player").GetComponent<Player>().Pay(repairCosts))
                    {
                        if (health < maxHealth)
                        {
                            InstatiateRepairGO();
                            health += repairPrSec;
                        }
                        else
                        {
                            health = maxHealth;
                            RemoveRepairGO();
                        }
                        timeSinceLastRepair = Time.time;
                    } else
                    {
                        RemoveRepairGO();
                    }
                }
            } else
            {
                RemoveRepairGO();
            }
        }
    }

    public GameObject LookForRessources()
    {
        GameObject[] ressourceGOs = GameObject.FindGameObjectsWithTag("Ressources");
        float shortestDis = float.MaxValue;
        GameObject closestRes = null;
        foreach(GameObject resGO in ressourceGOs)
        {
            float dis = Vector3.Distance(resGO.transform.position, transform.position);
            if (dis < ressourceSearchArea && dis < shortestDis)
            {
                shortestDis = dis;
                closestRes = resGO;
            }
        }
        return closestRes;
    }

    public void GoPlacePlate(GameObject turretSpot)
    {
        target = turretSpot;
        seeker.StartPath(transform.position, target.transform.position);
        state = State.PlacingPlate;
    }

    public void GoHome()
    {
        if (refinery == null)
            return;
        seeker.StartPath(transform.position, refinery.transform.GetChild(0).transform.position);
        state = State.ReturnHome;
    }

    public void GoToPosition(Vector3 pos)
    {
        seeker.StartPath(transform.position, pos);
        state = State.Driving;
    }

    public void GoToRessource(GameObject ressource)
    {
        target = ressource;
        state = State.GoToRessource;
        seeker.StartPath(transform.position, ressource.transform.position);
    }

    public void Damage(float d)
    {
        health -= d;
        if (health <= 0f)
            Destroy(this.gameObject);
    }

    public void AddHealth(float h)
    {
        health += h;
    }

    public void AddSpeed(float s)
    {
        GetComponent<AIPath>().speed += s;
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

    public string[] InfoText()
    {
        string[] info = new string[3];
        info[0] = "Harvester";
        info[1] = "Health : " + health + "/" + maxHealth;
        info[2] = "Ressources : " + ressources + "/" + ressourceCapacity;
        return info;
    }
}
