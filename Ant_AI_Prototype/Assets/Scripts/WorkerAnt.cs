using UnityEngine;
using System.Collections;

public class WorkerAnt : AntNew {

    #region private
    [SerializeField]
    [Tooltip("The distance the ant will discover resources")]
    private float resourceSearchRadius;
    [SerializeField]
    [Tooltip("The minimum distance to the position the ant will scout.")]
    private float minScoutDistance;
    [SerializeField]
    [Tooltip("The maximum distance to the position the ant will scout.")]
    private float maxScoutDistance;
    [SerializeField]
    [Tooltip("The maximum time the ant will be in a scout mode.")]
    private float maxScoutTime;
    #endregion
    #region public
    public float MaxScoutTime { get { return maxScoutTime; } }
    #endregion

    // Use this for initialization
    void Start () {
        Init();
        // Construct states
        IdleStateWorker idleState = new IdleStateWorker(this);
        ScoutStateWorker scoutState = new ScoutStateWorker(this);
        GoingHomeWorker goingHomeState = new GoingHomeWorker(this);
        // Add states to the fsm system
        fsm.AddState(idleState);
        fsm.AddState(scoutState);
        fsm.AddState(goingHomeState);
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

    /// <summary>
    /// Returns a random position based on the two variables: minScoutDistance and maxScoutDistance.
    /// </summary>
    /// <returns></returns>
    public Vector3 GetRandomNearLocation()
    {
        Vector3 ranVector = new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f));
        if (ranVector.magnitude == 0.0f)
            ranVector = Vector3.right;
        ranVector.Normalize();
        ranVector *= Random.Range(minScoutDistance, maxScoutDistance);

        Vector3 ranPos = transform.position + ranVector;

        RaycastHit hit;

        if (Physics.Raycast(ranPos + new Vector3(0.0f, 10.0f, 0.0f), Vector3.down, out hit))
        {
            if (hit.transform.gameObject.name != "Ground")
            {
                ranPos = transform.position;
            }
        }

        return ranPos;
    }
}
