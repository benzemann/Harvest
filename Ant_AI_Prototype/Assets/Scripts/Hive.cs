using UnityEngine;
using System.Collections;

public class Hive : MonoBehaviour {

    public GameObject ant;
    public GameObject warriorAnt;
    public float workerSpawnTime;
    public float warriorSpawnTime;
    public int maxActiveAnts;
    public int maxActiveWarriors;
    public float ressources;
    int activeAnts = 0;
    int activeWarriors = 0;
    float timeSinceLastWorker = 0.0f;
    float timeSinceLastWarrior = 0.0f;
    Feromones feromones;
	// Use this for initialization
	void Start () {
        feromones = GameObject.Find("FeromoneHandler").GetComponent<Feromones>();
	}
	
	// Update is called once per frame
	void Update () {
        timeSinceLastWorker += Time.deltaTime;
        timeSinceLastWarrior += Time.deltaTime;
        if(timeSinceLastWorker > workerSpawnTime)
        {
            SpawnWorker();
            timeSinceLastWorker = 0.0f;
        }
        if(timeSinceLastWarrior > warriorSpawnTime)
        {
            SpawnWarrior();
            timeSinceLastWarrior = 0.0f;
        }
        int x, y;
        feromones.GetCurrentGridCoords(transform.position, out x, out y);
        feromones.SetDefendFeromone(x, y, 0.0f);
	}

    void SpawnWorker()
    {
        if(activeAnts < maxActiveAnts)
        {
            GameObject antGO = Instantiate(ant, transform.position + Vector3.up * 0.5f, Quaternion.identity) as GameObject;
            antGO.GetComponent<Ant>().SetHive(this.gameObject);
            activeAnts += 1;
        } 
    }

    void SpawnWarrior()
    {
        if(activeWarriors < maxActiveWarriors)
        {
            GameObject warrior = Instantiate(warriorAnt, transform.position + Vector3.up * 0.5f, Quaternion.identity) as GameObject;
            warrior.GetComponent<WarriorAnt>().SetHive(this.gameObject);
            activeWarriors += 1;
        }
    }

    public void EnterHive(float returnRessources)
    {
        activeAnts -= 1;
        ressources += returnRessources;
    }

    public void EnterWarriorHive()
    {
        activeWarriors -= 1;
    }
}
