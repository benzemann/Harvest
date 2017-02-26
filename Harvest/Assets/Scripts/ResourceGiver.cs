using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ResourceStorage))]
public class ResourceGiver : MonoBehaviour {

    [SerializeField, Tooltip("The amount of resource to be given")]
    private int giveAmount;
    [SerializeField, Tooltip("The time in seconds between each give")]
    private float giveSpeed;
    [SerializeField, Tooltip("How far the object needs to be from a reciever to give")]
    private float giveDistance;
    [SerializeField, Tooltip("How fast the object will rotate toward a reciever when trying to give to it")]
    private float rotationTowardsRecieverSpeed;

    private float timeSinceLastGive;
    private bool firstTimeGive = true;

    public ResourceReciever TargetReciever { get; set; }
	
	// Update is called once per frame
	void Update () {
		if(TargetReciever != null)
        {
            if (Vector3.Distance(transform.position, TargetReciever.transform.position) <= giveDistance)
            {
                if (GetComponent<AgentController>() != null)
                {
                    GetComponent<AgentController>().Stop();
                }
                // Rotate towards resource
                float step = rotationTowardsRecieverSpeed * Time.deltaTime;
                Vector3 targetDir = TargetReciever.transform.position - this.transform.position;
                targetDir = new Vector3(targetDir.x, this.transform.position.y, targetDir.z);
                if (Vector3.Angle(targetDir, this.transform.forward) < 5f)
                {
                    GiveResource();
                }
                else
                {
                    // Need more rotation
                    Vector3 newDir = Vector3.RotateTowards(this.transform.forward, targetDir, step, 0.0f);
                    this.transform.rotation = Quaternion.LookRotation(newDir);
                }

            }
        }
	}

    private void GiveResource()
    {
        if (TargetReciever == null)
            return;

        if(GetComponent<ResourceStorage>().CurrentStorage <= 0)
        {
            TargetReciever = null;
            return;
        }

        if (firstTimeGive == true)
        {
            firstTimeGive = false;
            timeSinceLastGive = Time.time;
            return;
        }

        if (Time.time - timeSinceLastGive < giveSpeed)
        {
            return;
        }
        timeSinceLastGive = Time.time;

        var amount = Mathf.Min(giveAmount, GetComponent<ResourceStorage>().CurrentStorage);
        int remainder;
        TargetReciever.GetComponent<ResourceReciever>().Store(amount, out remainder);

        GetComponent<ResourceStorage>().TakeResource(amount - remainder);
    }
}
