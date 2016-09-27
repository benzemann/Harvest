using UnityEngine;
using System.Collections;
using Pathfinding;

public class Ant : MonoBehaviour {

    public float ressourcesSearchRadius;
    public float minScoutDistance;
    public float maxScoutDistance;
    public float harvestTime;
    public float minScoutTime;
    public float maxScoutTime;
    public float changeOfScoutOnStart;
    public float harvestAmount;

    Feromones feromones;
    GameObject target;
    Seeker seeker;
    GameObject hive;

    int lastX;
    int lastY;
    int gridX;
    int gridY;

    float startScoutTime;
    float timeSinceLastTrail;
    float startHarvestTime;

    public float ressource = 0.0f;

    enum State { IDLE, FOLLOWTRAIL, WORKER, SCOUT, GOTORESSOURCE, HARVESTING, RETURNHOME };

    State state;

    // Use this for initialization
    void Start () {
        feromones = GameObject.Find("FeromoneHandler").GetComponent<Feromones>();
        seeker = GetComponent<Seeker>();
        state = State.IDLE;
    }

    public void OnPathComplete(Path p)
    {
        Debug.Log("Path calculated. Errors? " + p.error);
    }

    // Update is called once per frame
    void Update () {
        //feromones.GetCurrentGridCoords(transform.position, out gridX, out gridY);
        timeSinceLastTrail += Time.deltaTime;
        switch (state)
        {
            case State.IDLE:
                if (!CheckForRessources())
                {
                    if(Random.Range(0.0f,100.0f) <= changeOfScoutOnStart)
                    {
                        state = State.SCOUT;
                        startScoutTime = Time.time;
                    } else
                    {
                        state = State.WORKER;
                    }
                    PathComplete();
                }
                break;
            case State.GOTORESSOURCE:
                if (target == null)
                {
                    state = State.IDLE;
                    break;
                }
                if(Vector3.Distance(target.transform.position, transform.position) < 2.0f)
                {
                    state = State.HARVESTING;
                    startHarvestTime = Time.time;
                }
                break;
            case State.SCOUT:
                if (CheckForRessources())
                    break;
                break;
            case State.WORKER:
                if (CheckForRessources())
                    break;
                break;
            case State.HARVESTING:
                if(Time.time - startHarvestTime > harvestTime)
                {
                    feromones.GetCurrentGridCoords(transform.position, out lastX, out lastY);
                    state = State.RETURNHOME;
                    seeker.StartPath(transform.position, hive.transform.position);
                    if (target != null)
                    {
                        if(target.GetComponent<Ressource>().Harvest(harvestAmount))
                            ressource += harvestAmount;
                    }
                }
                break;
            case State.RETURNHOME:
                if(Vector3.Distance(transform.position, hive.transform.position) < 0.60f)
                {
                    hive.GetComponent<Hive>().EnterHive(ressource);
                    lastX = gridX;
                    lastY = gridY;
                    feromones.GetCurrentGridCoords(transform.position, out gridX, out gridY);
                    if (ressource > 0)
                        AddFeromoneTrail();
                    Destroy(this.gameObject);
                }
                break;
        }
	}

    public void PathComplete()
    {

        feromones.GetCurrentGridCoords(transform.position, out gridX, out gridY);
        if (state == State.SCOUT)
        {
            seeker.StartPath(transform.position, GetRandomNearLocation());
            if(Time.time - startScoutTime > minScoutTime)
            {
                state = State.WORKER;
            }
        }
        if (state == State.WORKER)
        {
            //Debug.Log(SmellForFeromoneTrail());
            if (!SmellForFeromoneTrail())
            {
                seeker.StartPath(transform.position, GetRandomNearLocation());
                state = State.SCOUT;
                startScoutTime = Time.time;
            }

            if(timeSinceLastTrail > maxScoutTime)
            {
                state = State.RETURNHOME;
                seeker.StartPath(transform.position, hive.transform.position);
            }
                
        }
        if( state == State.RETURNHOME)
        {
            if(ressource > 0.0f)
                AddFeromoneTrail();
        }
    }

    public void NextWaypoint()
    {
        lastX = gridX;
        lastY = gridY;

        feromones.GetCurrentGridCoords(transform.position, out gridX, out gridY);

        //Debug.Log(gridX + " " + gridY + " last: " + lastX + " " + lastY);

        if (state == State.WORKER)
        {
            SmellForFeromoneTrail();
        }
        if(state == State.RETURNHOME)
        {
            if (ressource > 0.0f)
                AddFeromoneTrail();
        }
    }

    Vector3 GetRandomNearLocation()
    {
        Vector3 ranVector = new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f));
        if (ranVector.magnitude == 0.0f)
            ranVector = Vector3.right;
        ranVector.Normalize();
        ranVector *= Random.Range(minScoutDistance, maxScoutDistance);

        Vector3 ranPos = transform.position + ranVector;

        RaycastHit hit;

        if (Physics.Raycast(ranPos + new Vector3(0.0f, 10.0f, 0.0f), Vector3.down, out hit))
        {
            if(hit.transform.gameObject.name != "Ground")
            {
                ranPos = transform.position;
            }
        }

        return ranPos;
    }

    bool CheckForRessources()
    {
        GameObject[] ressources = GameObject.FindGameObjectsWithTag("Ressources");
        foreach(GameObject res in ressources)
        {
            float dis = Vector3.Distance(res.transform.position, transform.position);
            if(dis < ressourcesSearchRadius)
            {
                state = State.GOTORESSOURCE;
                target = res;
                seeker.StartPath(transform.position, target.transform.position);
                return true;
            }
        }
        return false;
    }

    bool SmellForFeromoneTrail()
    {
        Vector3 feromoneTrail = feromones.GetCloseFeromoneTrail(gridX, gridY);
        if(feromoneTrail.x != -1000.0f)
        {
            //Debug.Log(feromoneTrail + " " + transform.position);
            seeker.StartPath(transform.position, feromoneTrail);
            timeSinceLastTrail = 0.0f;
            return true;
        }
        return false;
    }

    void AddFeromoneTrail()
    {
        feromones.AddFeromone(gridX, gridY, lastX, lastY, 1);
    }

    public void SetHive(GameObject h)
    {
        hive = h;
    }
    
}
