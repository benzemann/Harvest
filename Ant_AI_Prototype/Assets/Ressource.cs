using UnityEngine;
using System.Collections;

public class Ressource : MonoBehaviour {

    public float ressource;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	    if(ressource <= 0.0f)
        {
            Destroy(this.gameObject);
        }
	}
}
