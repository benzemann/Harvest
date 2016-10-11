using UnityEngine;
using System.Collections;

public class Harvester : MonoBehaviour {

    public float maxHealth;
    [Tooltip("The time to harvest 1 ressource")]
    public float harvestTime;
    [Tooltip("The time to unload 1 ressource")]
    public float unloadTime;
    public float ressourceCapacity;
    public float ressources;
    float health;
    float timeSinceLastHarvest;
    float timeSinceLastUnload;
    float timeSinceLastRepair;

    enum State { Driving, GoToRessource, Harvest, ReturnHome, PlacingPlate };

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
                state = State.Driving;
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
                    target.GetComponent<Ressource>().Harvest(1);
                    ressources += 1;
                    if(ressources >= ressourceCapacity)
                    {
                        if (refinery == null)
                            return;
                        GoHome();
                    }
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
                    if (health < maxHealth)
                        health += 0.5f;

                    timeSinceLastRepair = Time.time;
                }
            }
        }
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

    public string[] InfoText()
    {
        string[] info = new string[3];
        info[0] = "Harvester";
        info[1] = "Health : " + health + "/" + maxHealth;
        info[2] = "Ressources : " + ressources + "/" + ressourceCapacity;
        return info;
    }
}
