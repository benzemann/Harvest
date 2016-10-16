using UnityEngine;
using System.Collections;

public class RessourceRespawn : MonoBehaviour {

    public GameObject ressource;
    public float timeBetweenSpawn;
    float timeSinceLastSpawn;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if(Time.time - timeSinceLastSpawn > timeBetweenSpawn)
        {
            SpawnRessources();
            timeSinceLastSpawn = Time.time;
        }
	}

    void SpawnRessources()
    {
        Vector3 ranPos = new Vector3(transform.position.x + Random.Range(-transform.localScale.x * 0.5f, transform.localScale.x * 0.5f),
                                        0f,
                                        transform.position.z + Random.Range(-transform.localScale.z * 0.5f, transform.localScale.z * 0.5f));
        GameObject res = Instantiate(ressource, ranPos, Quaternion.Euler(-90f, 0f, 0f)) as GameObject;
        res.transform.GetChild(0).gameObject.GetComponent<Ressource>().shallGrow = true;
    }
}
