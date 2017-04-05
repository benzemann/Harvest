using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class AgentController : MonoBehaviour
{

    #region SerializedFields
    [Header("Movement variables")]
    [SerializeField, Tooltip("The speed of the agent")]
    float speed;
    [SerializeField, Tooltip("The mass, high mass equals a more 'heavy' agent (slower turn speed)")]
    float mass;
    [SerializeField, Tooltip("The max force applied to the agent. Higher max force will result in more acceleration and turning speed")]
    float maxForce;
    [SerializeField, Tooltip("The max degrees the gameobject can turn each frame.")]
    float maxDeltaAngle;
    [Header("Path variables")]
    [SerializeField, Tooltip("The time between each time the agent will try to calculate a new path, low repathRate may affect performance")]
    float repathRate;
    [SerializeField, Tooltip("The distance the agent will pick a new waypoint.")]
    float pickNextWaypointDistance;
    [Header("Push force variables")]
    [SerializeField, Tooltip("Whether this agent is pushed by other agents")]
    bool applyPushingForce;
    [SerializeField, Tooltip("The radius of influence of the pushing force")]
    float pushRadius;
    [SerializeField, Tooltip("How much the agent is pushed")]
    float pushWeight;
    #endregion
    #region Private
    NavMeshPath navMeshPath;
    Vector3[] path;
    int currentWaypoint;
    bool targetReached;
    Vector3 targetPos;
    Vector3 velocity;
    float timeSinceLastPath;
    bool canWalkThrough = false;
    #endregion
    #region Properties
    public float MaxForce { get { return maxForce; } set { maxForce = value; } }
    public float Speed { get { return speed; } set { speed = value; } }
    public float Mass { get { return mass; } set { mass = value; } }
    public Vector3 PushingForce { get; set; }
    public int NoOfPushers { get; set; }
    public float CurrentSpeed { get { return velocity.magnitude; } }
    public bool IsWalking {
        get {
            if (path != null)
                return true;
            return false;
        }
    }
    public float PushRadius { get { return pushRadius; } }
    public bool ApplyPushingForces { get { return applyPushingForce; } }
    #endregion

    // Use this for initialization
    void Start()
    {
        // Add this to the agent manager
        AgentManager.Instance.AddAgent(this);

        navMeshPath = new NavMeshPath();
    }

    /// <summary>
    /// Order the agent to go to specific position.
    /// </summary>
    /// <param name="tp">The target position</param>
    public void GoToPos(Vector3 tp)
    {
        Stop();
        targetPos = tp;

        NavMesh.CalculatePath(this.transform.position, targetPos, NavMesh.AllAreas, navMeshPath);

        if (navMeshPath.status == NavMeshPathStatus.PathComplete || navMeshPath.status == NavMeshPathStatus.PathPartial)
        {
            if(navMeshPath.corners.Length > 0)
            {
                path = new Vector3[navMeshPath.corners.Length + 1];
                for (int i = 0; i < navMeshPath.corners.Length; i++)
                {
                    path[i] = navMeshPath.corners[i];
                }
                path[navMeshPath.corners.Length] = targetPos;
            } else
            {
                path = new Vector3[] { targetPos };
            }
        } else if (navMeshPath.status == NavMeshPathStatus.PathInvalid)
        {
            Debug.LogWarning("Could not calculate path");
        }
    }

    /// <summary>
    /// Moves the agent with the current velocity
    /// </summary>
    public void ApplyVelocity()
    {
        if (path == null) return;
        if (NoOfPushers > 0)
            PushingForce /= NoOfPushers;
        NoOfPushers = 0;
       
        transform.Translate(PushingForce, Space.World);
        PushingForce = Vector3.zero;
        if (velocity != Vector3.zero)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(velocity);
            Quaternion restrictedRotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, maxDeltaAngle);
            transform.rotation = restrictedRotation;
        }

        Vector3 finalVelocity = transform.forward * velocity.magnitude * Time.deltaTime * speed;

        if (finalVelocity.magnitude > speed)
            finalVelocity = finalVelocity.normalized * speed;

        if (finalVelocity.magnitude > 0f)
        {
            transform.Translate(finalVelocity, Space.World);
        }
        
    }

    /// <summary>
    /// Calculate the velocity of the agent.
    /// </summary>
    public void CalculateVelocity()
    {
        // Ensures that there is a path
        if (path == null || path.Length == 0)
        {
            velocity = Vector3.zero;
            return;
        }

        // Get the current xz position of the agent
        Vector3 currentPosition = this.transform.position;

        // Check if we need to go to next waypoint
        if (currentWaypoint <= path.Length - 1)
        {
            var dis = XZDistance(currentPosition, path[currentWaypoint]);
            // Check if agent is close enough to waypoint
            if (dis <= pickNextWaypointDistance)
            {
                currentWaypoint++;
                // Check if path is complete
                if (currentWaypoint >= path.Length)
                {
                    OnTargetReached();
                    return;
                }
            }
        }

        // Calculate steering
        Vector3 desiredVelocity = Vector3.Normalize(path[currentWaypoint] - currentPosition) * speed;
        Vector3 steering = desiredVelocity - velocity;
        if (steering.sqrMagnitude > maxForce)
            steering = Vector3.Normalize(steering) * maxForce;
        
        steering = steering / mass;

        // Calculate velocity
        velocity = velocity + steering;
    }

    /// <summary>
    /// Calculate push forcing based on close agents
    /// </summary>
    /// <param name="neighborAgents">A list of close neighborhood agents</param>
    public void CalculatePushForces(List<AgentController> neighborAgents)
    {
        foreach (AgentController agent in neighborAgents)
        {
            if (agent != this)
            {
                var d = Vector3.Distance(this.transform.position, agent.transform.position);
                
                Vector3 pushForce = this.transform.position - agent.transform.position;
                pushForce = pushForce.normalized;
                pushForce *= (pushRadius - d);
                pushForce = new Vector3(pushForce.x, 0f, pushForce.z);
                PushingForce += pushForce * Time.deltaTime * pushWeight;
                NoOfPushers++;
                
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
        // Remove path
        path = null;
        currentWaypoint = 0;
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

    private void OnDestroy()
    {
        AgentManager.Instance.RemoveAgent(this);
    }

}
