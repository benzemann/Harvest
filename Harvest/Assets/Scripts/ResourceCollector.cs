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
    [SerializeField, Tooltip("These particle systems will play when harvesting")]
    private ParticleSystem[] harvestParticles;
    [SerializeField, Tooltip("This sound is played when harvesting")]
    private AudioSource harvestSound;

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
                Vector3 targetDir = new Vector3(_targetResource.transform.position.x, this.transform.position.y, _targetResource.transform.position.z) - this.transform.position;
                targetDir = new Vector3(targetDir.x, 0f, targetDir.z);
                if(Vector3.Angle(targetDir, this.transform.forward) < 5f)
                {
                    if (HarvestResource())
                    {
                        if (harvestParticles.Length > 0)
                        {
                            if (harvestParticles[1].isStopped)
                            {
                                for (int i = 0; i < harvestParticles.Length; i++)
                                {
                                    harvestParticles[i].Play();
                                }
                            }
                        } 
                        if(harvestSound != null && !harvestSound.isPlaying)
                        {
                            harvestSound.Play();
                        }
                    } 
                } else
                {
                    // Need more rotation
                    Vector3 newDir = Vector3.RotateTowards(this.transform.forward, targetDir, step, 0.0f);
                    this.transform.rotation = Quaternion.LookRotation(newDir);
                }
                
            }
        } else
        {
            if (harvestParticles.Length > 0)
            {
                if (harvestParticles[1].isPlaying)
                {
                    for (int i = 0; i < harvestParticles.Length; i++)
                    {
                        harvestParticles[i].Stop();
                    }
                }
            }
            if (harvestSound != null && harvestSound.isPlaying)
            {
                harvestSound.Stop();
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
