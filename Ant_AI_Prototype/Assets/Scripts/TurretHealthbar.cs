using UnityEngine;
using System.Collections;

public class TurretHealthbar : MonoBehaviour {

	public Turret turret;

	// Use this for initialization
	void Start () {
	
	
	
	}
	
	// Update is called once per frame
	void Update () {
	
	float currentHealth = turret.health / turret.maxHealth;

	transform.localScale = new Vector3(currentHealth,transform.localScale.y,transform.localScale.z);

	}
}
