using UnityEngine;
using System.Collections;

public class WarriorAnt : MonoBehaviour {

    Feromones feromones;
    GameObject target;
    Seeker seeker;
    GameObject hive;

    public float defendFeromoneAmount;

    int lastX;
    int lastY;
    int gridX;
    int gridY;
    bool goingOut = true;

    enum State { IDLE, PATROLING, RETURNHOME };

    State state;

    // Use this for initialization
    void Start () {
        feromones = GameObject.Find("FeromoneHandler").GetComponent<Feromones>();
        seeker = GetComponent<Seeker>();
        state = State.IDLE;
        feromones.GetCurrentGridCoords(transform.position, out gridX, out gridY);
    }
	
	// Update is called once per frame
	void Update () {
        switch (state)
        {
            case State.IDLE:
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
                
                break;
            case State.RETURNHOME:
                if (Vector3.Distance(transform.position, hive.transform.position) < 0.60f)
                {
                    hive.GetComponent<Hive>().EnterWarriorHive();
                    Destroy(this.gameObject);
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
                AddDefendTrail();
                if (!CheckForProtectionTrail())
                {
                    state = State.IDLE;
                } 
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
}
