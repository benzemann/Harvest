using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ResourceCollector : MonoBehaviour {

    [SerializeField, Tooltip("The amount of resource per harvest")]
    private int harvestAmount;
    [SerializeField, Tooltip("The time in seconds between each harvest")]
    private float harvestSpeed;
    [SerializeField, Tooltip("How far the object needs to be from a resource to harvest it")]
    private float harvestDistance;
    [SerializeField, Tooltip("How fast the object will rotate toward a resource when trying to harvest it")]
    private float rotationTowardsResourceSpeed;

    private Resource _targetResource;
    private float timeSinceLastHarvest;
    private bool firstTimeHarvesting;

    public Resource Target {
        get {
            return _targetResource;
        }
        set {
            firstTimeHarvesting = true;
            _targetResource = value;
        }
    }

    public void Update()
    {
        if(_targetResource != null)
        {
            if(Vector3.Distance(transform.position, _targetResource.transform.position) <= harvestDistance)
            {
                if(GetComponent<AgentController>() != null)
                {
                    GetComponent<AgentController>().Stop();
                }
                // Rotate towards resource
                float step = rotationTowardsResourceSpeed * Time.deltaTime;
                Vector3 targetDir = _targetResource.transform.position - this.transform.position;
                targetDir = new Vector3(targetDir.x, this.transform.position.y, targetDir.z);
                if(Vector3.Angle(targetDir, this.transform.forward) < 5f)
                {
                    HarvestResource();
                } else
                {
                    // Need more rotation
                    Vector3 newDir = Vector3.RotateTowards(this.transform.forward, targetDir, step, 0.0f);
                    this.transform.rotation = Quaternion.LookRotation(newDir);
                }
                
            }
        }
    }

    public bool HarvestResource()
    {
        if (_targetResource == null)
            return false;

        if(firstTimeHarvesting == true)
        {
            firstTimeHarvesting = false;
            timeSinceLastHarvest = Time.time;
            return true;
        }

        if (Time.time - timeSinceLastHarvest < harvestSpeed)
        {
            return true;
        }
        timeSinceLastHarvest = Time.time;

        var amount = harvestAmount;

        if (GetComponent<ResourceStorage>() != null)
        {
            var remainingStorage = GetComponent<ResourceStorage>().RemainingStorageSpace;
            amount = Mathf.Min(remainingStorage, harvestAmount);
        }

        int harvest = _targetResource.Harvest(amount);

        if(GetComponent<ResourceStorage>() != null)
        {
            int remainder;
            if(GetComponent<ResourceStorage>().StoreResource(harvest, out remainder))
            {
                return true;
            } else
            {
                return false;
            }
        }

        return true;
    }
}
