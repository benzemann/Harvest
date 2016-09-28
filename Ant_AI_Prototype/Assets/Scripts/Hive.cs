using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Hive : MonoBehaviour {

    public GameObject ant;
    public GameObject warriorAnt;
    public GameObject distressBeacon;
    public float breedingDelay;
    public float spawnDelay;
    public float workerWarriorRatio;
    public float mustersPatrolingRatio;
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
    public int musteredWarriors = 0;
    public float ressources = 0;
    public Text ressourceText;
    Feromones feromones;
    float timeSinceLastBreed;
    float timeSinceLastSpawn;
    bool lastSpawnWorker;
    List<GameObject> distressBeacons = new List<GameObject>();

	// Use this for initialization
	void Start () {
        feromones = GameObject.Find("FeromoneHandler").GetComponent<Feromones>();
	}
	
	// Update is called once per frame
	void Update () {
        CheckAntsStatus();
        BreedAnts();
        MusterWarriors();
        SendOutAttackers();
        SpawnAnts();
        int x, y;
        feromones.GetCurrentGridCoords(transform.position, out x, out y);
        feromones.SetDefendFeromone(x, y, 0.0f);
        if (ressourceText != null)
            ressourceText.text = "Hive ressources: " + ressources.ToString();
	}

    void SendOutAttackers()
    {
        foreach(GameObject beacon in distressBeacons)
        {
            if (musteredWarriors >= beacon.GetComponent<DistressBeacon>().GetDistressLevel())
            {
                for(int i = 0; i < beacon.GetComponent<DistressBeacon>().GetDistressLevel(); i++)
                {
                    GameObject attacker = Instantiate(warriorAnt, transform.position + Vector3.up * 0.5f, Quaternion.identity) as GameObject;
                    attacker.GetComponent<WarriorAnt>().init();
                    attacker.GetComponent<WarriorAnt>().SetHive(this.gameObject);
                    attacker.GetComponent<WarriorAnt>().SetStateToGoingToBeacon(beacon.transform.position);
                    activeWarriorAnts.Add(attacker);
                    musteredWarriors -= 1;
                    if (musteredWarriors <= 0)
                        break;
                }
                beacon.GetComponent<DistressBeacon>().IncrementDistressLevel();
            }
        }
    }

    void MusterWarriors()
    {
        if(distressBeacons.Count > 0)
        {
            int sumOfDistressLevel = 0;
            foreach(GameObject beacon in distressBeacons)
            {
                sumOfDistressLevel += beacon.GetComponent<DistressBeacon>().GetDistressLevel();
            }
            float ratio = 0.0f;
            if (activeWarriorAnts.Count != 0)
                ratio = musteredWarriors / activeWarriorAnts.Count;
            
            int availableWarriors = totalWarriorAnts - activeWarriorAnts.Count - musteredWarriors;
            
            while (availableWarriors > 0 && musteredWarriors <= sumOfDistressLevel && ratio < mustersPatrolingRatio)
            {
                availableWarriors -= 1;
                musteredWarriors += 1;
                if (activeWarriorAnts.Count != 0)
                    ratio = musteredWarriors / activeWarriorAnts.Count;
            }
        }
    }

    void SpawnAnts()
    {
        if(Time.time - timeSinceLastSpawn > spawnDelay)
        {
            
            if (totalWorkerAnts > activeWorkerAnts.Count)
            {
                SpawnWorker();
                timeSinceLastSpawn = Time.time;
                lastSpawnWorker = true;
                return;
            }
            int availableWarriors = totalWarriorAnts - (activeWarriorAnts.Count + musteredWarriors);
            if (availableWarriors > 0)
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

            if(ratio < workerWarriorRatio && totalWorkerAnts < maxWorkerAnts)
            {
                if(ressources >= workerCost || totalWorkerAnts < freeWorkers)
                    BreedWorker();
            } else if (totalWarriorAnts < maxWarriorAnts)
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
        GameObject antGO = Instantiate(ant, transform.position + Vector3.up * 0.5f, Quaternion.identity) as GameObject;
        antGO.GetComponent<Ant>().SetHive(this.gameObject);
        activeWorkerAnts.Add(antGO);
    }

    void SpawnWarrior()
    {

        GameObject warrior = Instantiate(warriorAnt, transform.position + Vector3.up * 0.5f, Quaternion.identity) as GameObject;
        warrior.GetComponent<WarriorAnt>().SetHive(this.gameObject);
        activeWarriorAnts.Add(warrior);

    }

    public void EnterHive(float returnRessources)
    {
        ressources += returnRessources;
    }

    public void EnterWarriorHive()
    {
        
    }

    public void PutDownDistressBeacon(Vector3 pos)
    {
        foreach (GameObject b in distressBeacons)
            if (Vector3.Distance(b.transform.position, pos) < 2.0f)
                return;
        GameObject beacon = Instantiate(distressBeacon, pos, Quaternion.identity) as GameObject;
        beacon.GetComponent<DistressBeacon>().SetHive(this.gameObject);
        distressBeacons.Add(beacon);
    }

    public void RemoveDistressBeacon(GameObject beacon)
    {
        distressBeacons.Remove(beacon);
        Destroy(beacon);
    }

    public List<GameObject> GetDistressBeacons()
    {
        return distressBeacons;
    }
}
