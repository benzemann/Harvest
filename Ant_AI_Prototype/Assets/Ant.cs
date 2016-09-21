using UnityEngine;
using System.Collections;
using Pathfinding;

public class Ant : MonoBehaviour {

    public float ressourcesSearchRadius;
    public float minScoutDistance;
    public float maxScoutDistance;
    public float harvestTime;

    Feromones feromones;
    GameObject target;
    Seeker seeker;
    GameObject hive;

    int lastX;
    int lastY;
    int gridX;
    int gridY;

    float startHarvestTime;

    enum State { IDLE, FOLLOWTRAIL, SCOUT, GOTORESSOURCE, HARVESTING, RETURNHOME };

    State state;

    // Use this for initialization
    void Start () {
        feromones = GameObject.Find("FeromoneHandler").GetComponent<Feromones>();
        hive = GameObject.Find("Hive");
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
        switch (state)
        {
            case State.IDLE:
                if (!CheckForRessources())
                {
                    state = State.SCOUT;
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
            case State.HARVESTING:
                if(Time.time - startHarvestTime > harvestTime)
                {
                    feromones.GetCurrentGridCoords(transform.position, out lastX, out lastY);
                    state = State.RETURNHOME;
                    seeker.StartPath(transform.position, hive.transform.position);
                }
                break;
            case State.RETURNHOME:
                if(Vector3.Distance(transform.position, hive.transform.position) < 0.75f)
                {
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
            //Debug.Log(SmellForFeromoneTrail());
            if(!SmellForFeromoneTrail())
                seeker.StartPath(transform.position, GetRandomNearLocation());
        }
        if( state == State.RETURNHOME)
        {
            AddFeromoneTrail();
        }
    }

    public void NextWaypoint()
    {
        lastX = gridX;
        lastY = gridY;

        feromones.GetCurrentGridCoords(transform.position, out gridX, out gridY);

        //Debug.Log(gridX + " " + gridY + " last: " + lastX + " " + lastY);

        if (state == State.SCOUT)
        {
            SmellForFeromoneTrail();
        }
        if(state == State.RETURNHOME)
        {
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
        return transform.position + ranVector;
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
            Debug.Log(feromoneTrail + " " + transform.position);
            seeker.StartPath(transform.position, feromoneTrail);

            return true;
        }
        return false;
    }

    void AddFeromoneTrail()
    {
        feromones.AddFeromone(gridX, gridY, lastX, lastY, 1);
    }
    
}
