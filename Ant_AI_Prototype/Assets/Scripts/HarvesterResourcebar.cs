using UnityEngine;
using System.Collections;

public class HarvesterResourcebar : MonoBehaviour {

	public Harvester harvester;

	// Use this for initialization
	void Start () {
	
	
	
	}
	
	// Update is called once per frame
	void Update () {
	
	float currentResources = harvester.ressources / harvester.ressourceCapacity;

	transform.localScale = new Vector3(currentResources,transform.localScale.y,transform.localScale.z);

	}
}
