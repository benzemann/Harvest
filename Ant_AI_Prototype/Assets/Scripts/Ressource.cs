using UnityEngine;
using System.Collections;

public class Ressource : MonoBehaviour {

    public float maxRessource;
    public float ressource;
    public bool shallGrow;
    public float growthTime;
    public float startHeight;
    public float endHeight;
    float timeSinceStart;
	// Use this for initialization
	void Start () {
        if (!shallGrow)
            ressource = maxRessource;
        else
            transform.position = new Vector3(transform.position.x, startHeight, transform.position.z);
    }
	
	// Update is called once per frame
	void Update () {
        if (shallGrow)
            timeSinceStart += Time.deltaTime;
	    if(shallGrow && timeSinceStart <= growthTime)
        {
            ressource += (maxRessource / growthTime) * Time.deltaTime;
        } else if(shallGrow)
        {
            ressource += (maxRessource / growthTime) * Time.deltaTime;
            shallGrow = false;
        }
        transform.position = new Vector3(transform.position.x, startHeight * (1.0f - ((ressource) / (maxRessource))) + endHeight * ((ressource) / (maxRessource)), transform.position.z);
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
        if (ressource < 1.0f && !shallGrow)
        {
            Destroy(this.transform.parent.gameObject);
        } 
        return true;
    }
}
