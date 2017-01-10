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
    Transform target;
    Vector3 velocity;
    #endregion
    #region Public
    public float maxForce;
    public float maxSpeed;
    public float mass;
    public float pickNextWaypointDistance;
    #endregion


    // Use this for initialization
    void Start()
    {
        // Add this to the flocking controller
        FlockingController.Instance.AddAgent(this);
    }

    void Awake()
    {
        
        // Get the seeker component
        seeker = GetComponent<Seeker>();
        // Add the OnPathComplete method as a callback
        seeker.pathCallback += OnPathComplete;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("r"))
        {
            // ----------- Debug
            target = GameObject.Find("target").transform;
            GetComponent<Seeker>().StartPath(transform.position, target.position);
            // ----------- Debug end
        }
    }

    /// <summary>
    /// Moves the agent with the current velocity
    /// </summary>
    public void ApplyVelocity()
    {
        transform.Translate(velocity * Time.deltaTime, Space.World);
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

        // Calculate steering
        Vector3 desiredVelocity = Vector3.Normalize(vPath[currentWaypoint] - currentPosition) * maxSpeed;
        Vector3 steering = desiredVelocity - velocity;
        if (steering.sqrMagnitude > maxForce)
            steering = Vector3.Normalize(steering) * maxForce;
        steering = steering / mass;

        // Calculate velocity
        velocity = velocity + steering;
        if (velocity.sqrMagnitude > maxSpeed)
            velocity = Vector3.Normalize(velocity) * maxSpeed;

        // Check if we need to go to next waypoint
        if (currentWaypoint <= vPath.Count - 1)
        {
            // Check if agent is close enough to waypoint
            if (XZDistance(currentPosition, vPath[currentWaypoint]) <= pickNextWaypointDistance)
            {
                currentWaypoint++;
                // Check if path is complete
                if (currentWaypoint >= vPath.Count)
                {
                    OnTargetReached();
                }
            }
        }
    }

    /// <summary>
    /// Is called when target is reached
    /// </summary>
    void OnTargetReached()
    {
        // Release and remove path
        path.Release(this);
        path = null;
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

    /// <summary>
    /// Draw Gizmos for debugging
    /// </summary>
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(velocity != null)
            Gizmos.DrawLine(this.transform.position, this.transform.position + velocity);
    }
}
