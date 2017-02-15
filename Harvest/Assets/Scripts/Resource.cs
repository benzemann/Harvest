using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour {
    [SerializeField]
    private int startResource;

    private int remainingResource;

	// Use this for initialization
	void Start () {
        remainingResource = startResource;
        ResourceManager.Instance.AddResource(this);
	}
	
	// Update is called once per frame
	void Update () {
        
        if (remainingResource <= 0)
            Destroy(this.gameObject);

    }

    public int Harvest(int amount)
    {
        if(amount <= remainingResource)
        {
            remainingResource -= amount;
            return amount;
        } else
        {
            return remainingResource;
        }
    }

    private void OnDestroy()
    {
        ResourceManager.Instance.RemoveResource(this);
    }
}
