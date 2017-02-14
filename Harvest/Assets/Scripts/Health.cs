using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

    [SerializeField, Tooltip("The max health of the object")]
    private float maxHealth;
    
    private float health;

    public float Percentage { get { return health / maxHealth; } }
    public float Value { get { return health; } }

    // Use this for initialization
    void Start () {
        health = maxHealth;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Damage(float value)
    {
        health -= value;
    }
}
