using UnityEngine;
using System.Collections;

public class WorkerAnt : AntNew {

    [SerializeField]
    float resourceSearchRadius;

	// Use this for initialization
	void Start () {
        Init();
        // Construct states
        IdleStateWorker idleState = new IdleStateWorker(this);
        ScoutStateWorker scoutState = new ScoutStateWorker(this);
        // Add states to the fsm system
        fsm.AddState(idleState);
        fsm.AddState(scoutState);
    }
	
	// Update is called once per frame
	void Update () {
        UpdateFSM();
        UpdateAnimator();
    }

    /// <summary>
    /// Updates the animator component.
    /// </summary>
    void UpdateAnimator()
    {
        float currentSpeed = agentController.CurrentSpeed;
        anim.SetFloat("Speed", currentSpeed);
    }

    /// <summary>
    /// Checks if any resource is close. Optional out gameobject of the closest resource.
    /// </summary>
    /// <param name="closeResource">The closest resource if any.</param>
    /// <returns></returns>
    public bool IsResourcesClose(out GameObject closeResource)
    {
        GameObject[] resources = GameObject.FindGameObjectsWithTag("Ressources");
        closeResource = null;
        bool isResourceClose = false;
        float closestDis = float.MaxValue;
        foreach (GameObject res in resources)
        {
            float dis = Vector3.Distance(this.transform.position, res.transform.position);
            if(dis <= resourceSearchRadius)
            {
                isResourceClose = true;
                if (dis < closestDis)
                    closeResource = res;
            }
        }
        return isResourceClose;
    }
}
