using UnityEngine;
using System.Collections;

public class RefineryHealthbar1 : MonoBehaviour {

	public Refinery refinery;

	// Use this for initialization
	void Start () {
	
	
	
	}
	
	// Update is called once per frame
	void Update () {
	
	float currentHealth = refinery.health / refinery.maxHealth;

	transform.localScale = new Vector3(currentHealth,transform.localScale.y,transform.localScale.z);

	}
}
