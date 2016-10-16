using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour {

    public float costs;
    public GameObject[] upgrades;
    public string textOnButton;
	// Use this for initialization
	void Start () {
        int i = 0;
        foreach(GameObject upgrade in upgrades)
        {
            GameObject upgradeGo = Instantiate(upgrade, Vector3.zero, Quaternion.identity) as GameObject;
            upgrades[i] = upgradeGo;
            i++;
        }
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

}
