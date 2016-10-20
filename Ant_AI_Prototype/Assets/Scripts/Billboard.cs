using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 v3 = Camera.main.transform.position - transform.position;
        v3.x = 0.0f;
        if(Vector3.Dot(v3.normalized, Vector3.forward) < 0f)
            transform.rotation = Quaternion.LookRotation(-v3);

        //transform.LookAt(Camera.main.transform.position);
	}
}
