using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hive : MonoBehaviour {

    public GameObject ant;
    public GameObject warriorAnt;
    public float breedingDelay;
    public float spawnDelay;
    public float workerWarriorRatio;
    public float workerCost;
    public float warriorCost;
    public int maxWorkerAnts;
    public int maxWarriorAnts;
    public int freeWorkers;
    public int freeWarriors;
    List<GameObject> activeWorkerAnts = new List<GameObject>();
    List<GameObject> activeWarriorAnts = new List<GameObject>();
    public int totalWorkerAnts = 0;
    public int totalWarriorAnts = 0;
    public float ressources = 0;
    Feromones feromones;
    float timeSinceLastBreed;
    float timeSinceLastSpawn;
    bool lastSpawnWorker;
	// Use this for initialization
	void Start () {
        feromones = GameObject.Find("FeromoneHandler").GetComponent<Feromones>();
	}
	
	// Update is called once per frame
	void Update () {
        CheckAntsStatus();
        BreedAnts();
        SpawnAnts();
        int x, y;
        feromones.GetCurrentGridCoords(transform.position, out x, out y);
        feromones.SetDefendFeromone(x, y, 0.0f);
	}

    void SpawnAnts()
    {
        if(Time.time - timeSinceLastSpawn > spawnDelay)
        {
            if(totalWorkerAnts > activeWorkerAnts.Count && (!lastSpawnWorker || (lastSpawnWorker && totalWarriorAnts <= activeWarriorAnts.Count)))
            {
                SpawnWorker();
                timeSinceLastSpawn = Time.time;
                lastSpawnWorker = true;
                return;
            }
            if(totalWarriorAnts > activeWarriorAnts.Count)
            {
                SpawnWarrior();
                timeSinceLastSpawn = Time.time;
                lastSpawnWorker = false;
            }
        }
    }

    void BreedAnts()
    {
        if(Time.time - timeSinceLastBreed > breedingDelay)
        {
            
            float ratio = 0.0f;
            if (totalWarriorAnts == 0)
                ratio = totalWorkerAnts;
            else
                ratio = totalWorkerAnts / totalWarriorAnts;

            if(ratio <= workerWarriorRatio && totalWorkerAnts < maxWorkerAnts)
            {
                if(ressources >= workerCost || totalWorkerAnts < freeWorkers)
                    BreedWorker();
            } else
            {
                if (ressources >= warriorCost || totalWarriorAnts < freeWarriors)
                    BreedWarrior();
            }
        }
    }

    void CheckAntsStatus()
    {
        for(int i = 0; i < activeWorkerAnts.Count; i++)
        {
            if (activeWorkerAnts[i] == null)
                activeWorkerAnts.RemoveAt(i);
        }
        for (int i = 0; i < activeWarriorAnts.Count; i++)
        {
            if (activeWarriorAnts[i] == null)
                activeWarriorAnts.RemoveAt(i);
        }
    }

    void BreedWorker()
    {
        if(totalWorkerAnts >= freeWorkers)
            ressources -= workerCost;
        totalWorkerAnts += 1;
        timeSinceLastBreed = Time.time;
    }

    void BreedWarrior()
    {
        if(totalWarriorAnts >= freeWarriors)
            ressources -= warriorCost;
        totalWarriorAnts += 1;
        timeSinceLastBreed = Time.time;
    }

    void SpawnWorker()
    {
        if(totalWorkerAnts < maxWorkerAnts)
        {
            GameObject antGO = Instantiate(ant, transform.position + Vector3.up * 0.5f, Quaternion.identity) as GameObject;
            antGO.GetComponent<Ant>().SetHive(this.gameObject);
            activeWorkerAnts.Add(antGO);
        } 
    }

    void SpawnWarrior()
    {
        if(totalWarriorAnts < maxWarriorAnts)
        {
            GameObject warrior = Instantiate(warriorAnt, transform.position + Vector3.up * 0.5f, Quaternion.identity) as GameObject;
            warrior.GetComponent<WarriorAnt>().SetHive(this.gameObject);
            activeWarriorAnts.Add(warrior);
        }
    }

    public void EnterHive(float returnRessources)
    {
        ressources += returnRessources;
    }

    public void EnterWarriorHive()
    {
        
    }
}
