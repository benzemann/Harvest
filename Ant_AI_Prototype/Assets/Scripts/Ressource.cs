using UnityEngine;
using System.Collections;

public class Ressource : MonoBehaviour {

    public float ressource;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    public bool Harvest(float amount)
    {
        if(ressource >= amount)
        {
            ressource -= amount;
        } else
        {
            return false;
        }
        if (ressource <= 0.0f)
        {
            Destroy(this.gameObject);
        }
        return true;
    }
}
