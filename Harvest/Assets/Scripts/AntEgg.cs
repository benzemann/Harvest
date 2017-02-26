using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ResourceStorage))]
public class AntEgg : MonoBehaviour {

    [SerializeField, Tooltip("The time until the egg hatches. This time starts when it is filled with resource and readytime is done")]
    private float hatchTime;
    [SerializeField, Tooltip("The time until the egg is ready. This time needs to go before hatch time.")]
    private float readyTime;
    [SerializeField, Tooltip("How much resource is needed for the egg to hatch")]
    private int resourceNeeded;
    [SerializeField, Tooltip("The prefab of the ant that will spawn from this egg")]
    private GameObject ant;

    private bool isReady;
    private ResourceStorage storage;
    private float timeAtInstatiation;
    private float timeAtReady;
	// Use this for initialization
	void Start () {
        ObjectManager.Instance.AddEgg(this.gameObject);
        storage = GetComponent<ResourceStorage>();
        timeAtInstatiation = Time.time;
	}
	
	// Update is called once per frame
	void Update () {

		if(storage.CurrentStorage >= resourceNeeded && Time.time - timeAtInstatiation >= readyTime)
        {
            isReady = true;
            timeAtReady = Time.time;
        }
        if (isReady && Time.time - timeAtReady >= hatchTime)
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
