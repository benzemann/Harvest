using UnityEngine;
using System.Collections;

public class Hive : MonoBehaviour {

    public GameObject ant;
    public float workerSpawnTime;
    public int maxActiveAnts;
    public float ressources;
    int activeAnts = 0;
    float timeSinceLastWorker = 0.0f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        timeSinceLastWorker += Time.deltaTime;
        if(timeSinceLastWorker > workerSpawnTime)
        {
            SpawnWorker();
            timeSinceLastWorker = 0.0f;
        }
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

    public void EnterHive(float returnRessources)
    {
        activeAnts -= 1;
        ressources += returnRessources;
    }
}
