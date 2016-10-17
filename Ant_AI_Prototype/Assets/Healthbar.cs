using UnityEngine;
using System.Collections;

public class Healthbar : MonoBehaviour {

	public Harvester harvester;

	// Use this for initialization
	void Start () {
	
	

	
	
	}
	
	// Update is called once per frame
	void Update () {
	
	float currentHealth = harvester.health / harvester.maxHealth;

	transform.localScale = new Vector3(currentHealth,transform.localScale.y,transform.localScale.z);

	}
}
