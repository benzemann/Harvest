using UnityEngine;
using System.Collections;

public class TurretSpot : MonoBehaviour {

    public Material dirtMat;
    public Material ironMat;
    public bool hasPlate = false;
    public bool hasTurret = false;
    GameObject turret;
	// Use this for initialization
	void Start () {
        if (hasPlate)
            GetComponent<Renderer>().material = ironMat;
        else
            GetComponent<Renderer>().material = dirtMat;
        
    }
	
	// Update is called once per frame
	void Update () {

        if (turret == null)
            hasTurret = false;
	
	}

    public void SetTurret(GameObject t)
    {
        turret = t;
    }

    public bool HasPlate()
    {
        return hasPlate;
    }

    public void AddPlate()
    {
        hasPlate = true;
        GetComponent<Renderer>().material = ironMat;
    }
}
