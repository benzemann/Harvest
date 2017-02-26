using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ResourceStorage))]
public class AntEgg : MonoBehaviour {

    [SerializeField, Tooltip("The time until the egg hatches")]
    private float hatchTime;
    [SerializeField, Tooltip("How much resource is needed for the egg to hatch")]
    private int resourceNeeded;
    [SerializeField, Tooltip("The prefab of the ant that will spawn from this egg")]
    private GameObject ant;

    private ResourceStorage storage;
    private float timeAtInstatiation;
	// Use this for initialization
	void Start () {
        ObjectManager.Instance.AddEgg(this.gameObject);
        storage = GetComponent<ResourceStorage>();
        timeAtInstatiation = Time.time;
	}
	
	// Update is called once per frame
	void Update () {

		if(storage.CurrentStorage >= resourceNeeded && Time.time - timeAtInstatiation >= hatchTime)
        {
            Instantiate(ant, this.transform.position, this.transform.rotation);
            Destroy(this.gameObject);
        }

    }

    private void OnDestroy()
    {
        ObjectManager.Instance.RemoveEgg(this.gameObject);
    }
}
