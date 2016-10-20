using UnityEngine;
using System.Collections;

public class HarvesterHealthbar : MonoBehaviour {

	public Harvester harvester;

	// Use this for initialization
	void Start () {
	
	
	
	}
	
	// Update is called once per frame
	void Update () {

        if (harvester == null)
        {
            GameObject harvesterGO = GameObject.Find("Harvester");
            if(harvesterGO != null)
                harvester = harvesterGO.GetComponent<Harvester>();
        }
            
	    float currentHealth = harvester.health / harvester.maxHealth;

	    transform.localScale = new Vector3(currentHealth,transform.localScale.y,transform.localScale.z);

	}
}
