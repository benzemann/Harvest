using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

public class AgentController : MonoBehaviour
{

    #region Private
    Path path;
    int currentWaypoint;
    bool targetReached;
    Seeker seeker;
    Vector3 targetPos;
    Vector3 velocity;
    Vector3 flockingForces;
    CharacterController controller;
    bool isStandingGround = false;
    float timeSinceLastPath;
    bool canWalkThrough = false;
    float speed;
    float mass;
    float maxForce;
    [SerializeField]
    float repathRate;
    #endregion
    #region Public
    public float MaxForce { get { return maxForce; } set { maxForce = value; } }
    public float Speed { get { return speed; } set { speed = value; } }
    public float Mass { get { return mass; } set { mass = value; } }
    public float pickNextWaypointDistance;
    public Vector3 PushingForce { get; set; }
    public bool IsStandingGround{
        get { return isStandingGround; }
        set { isStandingGround = value; }
    }
    public bool CanWalkThrough
    {
        get { return canWalkThrough; }
        set { canWalkThrough = value; }
    }
    public int NoOfPushers { get; set; }
    public float CurrentSpeed { get { return velocity.magnitude; } }
    #endregion

    // Use this for initialization
    void Start()
    {
        // Add this to the flocking controller
        FlockingController.Instance.AddAgent(this);
        maxForce = 1f;  
    }

    void Awake()
    {
        // Get the seeker component
        seeker = GetComponent<Seeker>();
        // Add the OnPathComplete method as a callback
        seeker.pathCallback += OnPathComplete;
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Order the agent to go to specific position.
    /// </summary>
    /// <param name="tp">The target position</param>
    public void GoToPos(Vector3 tp)
    {
        targetPos = tp;
        StartCoroutine(TryToSearchPath());
    }

    private IEnumerator TryToSearchPath()
    {
        while (true)
        {
            if (seeker.IsDone())
            {
                seeker.StartPath(transform.position, targetPos);
                yield return null;
            }
            yield return new WaitForSeconds(repathRate);
        }
          
    }

    /// <summary>
    /// Moves the agent with the current velocity
    /// </summary>
    public void ApplyVelocity()
    {
        if (NoOfPushers > 0)
            PushingForce /= NoOfPushers;
        NoOfPushers = 0;
        transform.Translate(PushingForce, Space.World);
        PushingForce = Vector3.zero;

        Vector3 finalVelocity = velocity * Time.deltaTime * speed;

        if(finalVelocity.magnitude > 0f)
        {
            transform.Translate(finalVelocity, Space.World);
        }

        if(velocity != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(velocity);
            transform.rotation = rotation;
        }
        //controller.SimpleMove(velocity);
    }

    /// <summary>
    /// Calculate the velocity of the agent.
    /// </summary>
    public void CalculateVelocity()
    {
        // Ensures that there is a path
        if (path == null || path.vectorPath == null || path.vectorPath.Count == 0)
        {
            velocity = Vector3.zero;
            return;
        }

        // Get the vector path
        List<Vector3> vPath = path.vectorPath;

        // Get the current xz position of the agent
        Vector3 currentPosition = this.transform.position;
        currentPosition.y = 0.0f;

        // Check if further ahead nodes are closer, if so increment currentwaypoint
        while (currentWaypoint < vPath.Count - 2)
        {
            float disToCurrent = Vector3.Distance(transform.position, vPath[currentWaypoint]);
            float disToNext = Vector3.Distance(transform.position, vPath[currentWaypoint + 1]);
            if (disToNext < disToCurrent)
            {
                currentWaypoint++;
            }
            else
            {
                break;
            }
        }

        // Check if we need to go to next waypoint
        if (currentWaypoint <= vPath.Count - 1)
        {
            var dis = Vector3.Distance(currentPosition, vPath[currentWaypoint]);
            // Check if agent is close enough to waypoint
            if (dis <= pickNextWaypointDistance)
            {
                currentWaypoint++;
                // Check if path is complete
                if (currentWaypoint >= vPath.Count)
                {
                    OnTargetReached();
                    return;
                }
            }
            else if (dis > 1.75f && Time.time - timeSinceLastPath >= 1f && GetComponent<Seeker>().IsDone())
            {
                GetComponent<Seeker>().StartPath(transform.position, targetPos);
                timeSinceLastPath = Time.time;
            }
        }

        // Calculate steering
        Vector3 desiredVelocity = Vector3.Normalize(vPath[currentWaypoint] - currentPosition) * speed;
        Vector3 steering = desiredVelocity - velocity;
        if (steering.sqrMagnitude > maxForce)
            steering = Vector3.Normalize(steering) * maxForce;
        steering = steering / mass;

        // Calculate velocity
        velocity = velocity + steering;
        velocity += flockingForces;

        // Make sure to mark area under the agent as walkable
        if (isStandingGround && velocity != Vector3.zero)
        {
            ReleaseGroundNodes();
        }
    }

    /// <summary>
    /// Updates the nodes under the agent to release them.
    /// </summary>
    public void ReleaseGroundNodes()
    {
        isStandingGround = false;
        Bounds bounds = GetComponent<Collider>().bounds;
        GraphUpdateObject guo = new GraphUpdateObject(bounds);
        //guo.modifyWalkability = true;
        //guo.setWalkability = true;
        AstarPath.active.UpdateGraphs(guo);
    }
    
    /// <summary>
    /// Calculate all flocking forces, allignment, cohesion, seperation.
    /// </summary>
    /// <param name="neighborAgents">A list of close agents</param>
    /// <param name="weight">The allignment weight.</param>
    public void ComputeFlockingForces(List<AgentController> neighborAgents, float allignmentWeight, float cohesionWeight, float seperationWeight)
    {
        flockingForces = Vector3.zero;

        Vector3 allignment = Vector3.zero;
        Vector3 cohesion = Vector3.zero;
        Vector3 seperation = Vector3.zero;

        int neighborCount = neighborAgents.Count;
        foreach(AgentController neighborAgent in neighborAgents)
        {
            allignment += neighborAgent.velocity;
            cohesion += neighborAgent.transform.position;
            seperation += neighborAgent.transform.position - this.transform.position;
        }

        if(neighborCount != 0)
        {
            allignment /= neighborCount;
            allignment = allignment.normalized;

            cohesion /= neighborCount;
            cohesion = cohesion - this.transform.position;
            cohesion = cohesion.normalized;

            seperation /= neighborCount;
            seperation = seperation.normalized;
            seperation *= -1f;
        }

        flockingForces = (allignment * allignmentWeight) + (cohesion * cohesionWeight) + (seperation * seperationWeight);
        flockingForces = new Vector3(flockingForces.x, 0.0f, flockingForces.z);

        foreach (AgentController agent in neighborAgents)
        {
            float d = Vector3.Distance(agent.transform.position, this.transform.position);
            if (d < 1f && agent != this)
            {
                if (!agent.IsStandingGround && !agent.CanWalkThrough)
                {
                    Vector3 pushForce = agent.transform.position - this.transform.position;
                    pushForce = pushForce.normalized;
                    pushForce *= (1f - d);
                    pushForce = new Vector3(pushForce.x, 0f, pushForce.z);
                    agent.PushingForce += pushForce;
                    agent.NoOfPushers++;
                } else if(!isStandingGround && !canWalkThrough)
                {
                    Vector3 pushForce = this.transform.position - agent.transform.position;
                    pushForce = pushForce.normalized;
                    pushForce *= (1f - d);
                    pushForce = new Vector3(pushForce.x, 0f, pushForce.z);
                    PushingForce += pushForce;
                    NoOfPushers++;
                    if(Time.time - timeSinceLastPath >= 0.25f && GetComponent<Seeker>().IsDone())
                    {
                        timeSinceLastPath = Time.time;
                        GetComponent<Seeker>().StartPath(transform.position, targetPos);
                    }
                    //GetComponent<Seeker>().StartPath(transform.position, -transform.forward * 2f);
                }
            }
        }
    }

    /// <summary>
    /// Is called when target is reached
    /// </summary>
    void OnTargetReached()
    {
        if (path == null)
            return;
        // Release and remove path
        path.Release(this);
        path = null;
    }

    /// <summary>
    /// Forces the agent to stop
    /// </summary>
    public void Stop()
    {
        if (path == null)
            return;
        OnTargetReached();
    }

    /// <summary>
    /// Marks the area under this agent as unwalkable, and cannot be pushed
    /// </summary>
    public void AvoidPushing()
    {
        if (isStandingGround)
            return;
        
        isStandingGround = true;
        Bounds bounds = GetComponent<Collider>().bounds;
        GraphUpdateObject guo = new GraphUpdateObject(bounds);
        guo.modifyWalkability = true;
        guo.setWalkability = false;
        AstarPath.active.UpdateGraphs(guo);
    }

    /// <summary>
    /// Is called when a path has been calculated
    /// </summary>
    /// <param name="p">The calculated path</param>
    void OnPathComplete(Path _p)
    {
        // Get the path as an ABPath
        ABPath p = _p as ABPath;

        // Check if p is valid
        if (p == null) throw new System.Exception("This function only handles ABPaths, do not use special path types");

        // Claim the new path
        p.Claim(this);

        // Check for any errors
        if (p.error)
        {
            Debug.LogError(p.error);
            p.Release(this);
            return;
        }

        // If there is a path, release it
        if (path != null)
            path.Release(this);

        // Set the path to the newly calculated path
        path = p;

        //Reset some variables
        targetReached = false;
        currentWaypoint = 0;
    }

    /// <summary>
    /// Returns the distance from a to b only in the X-Z plane.
    /// </summary>
    /// <param name="a">Start point</param>
    /// <param name="b">End point</param>
    /// <returns></returns>
    float XZDistance(Vector3 a, Vector3 b)
    {
        float dx = b.x - a.x;
        float dz = b.z - a.z;

        return dx * dx + dz * dz;
    }
/*
    /// <summary>
    /// Draw Gizmos for debugging
    /// </summary>
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(velocity != null)
            Gizmos.DrawLine(this.transform.position, this.transform.position + velocity);
        Gizmos.color = Color.blue;
        if(flockingForces != null)
            Gizmos.DrawLine(this.transform.position, this.transform.position + flockingForces);
    }*/
}
