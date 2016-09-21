using UnityEngine;
using System.Collections;

public class Hive : MonoBehaviour {

    public GameObject ant;
    public float workerSpawnTime;

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

        Instantiate(ant, transform.position + Vector3.up * 3.0f, Quaternion.identity);

    }
}
