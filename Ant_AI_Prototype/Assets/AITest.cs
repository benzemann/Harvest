using UnityEngine;
using System.Collections;
using Pathfinding;

public class AITest : MonoBehaviour {

    public GameObject target;

	// Use this for initialization
	void Start () {
        //Get a reference to the Seeker component we added earlier
        Seeker seeker = GetComponent<Seeker>();
        //Start a new path to the targetPosition, return the result to the OnPathComplete function
        seeker.StartPath(transform.position, target.transform.position, OnPathComplete);
    }

    public void OnPathComplete(Path p)
    {
        Debug.Log("Yay, we got a path back. Did it have an error? " + p.error);
    }
}
