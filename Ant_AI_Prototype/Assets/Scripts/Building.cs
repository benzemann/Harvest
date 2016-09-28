using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour {

    public float health;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Damage(float d)
    {
        health -= d;
        if (health <= 0)
            Destroy(this.gameObject);
    }
}
