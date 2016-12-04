using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlockingController : MonoBehaviour {

    #region Private
    List<AgentController> agents;
    private static FlockingController _instance;
    #endregion

    #region Public

    public float neighborhoodRadius;
    public float allignmentWeight;
    public float cohesionWeight;
    public float seperationWeight;

    /// <summary>
    /// Global accessable instance (Not singleton).
    /// </summary>
    public static FlockingController Instance
    {
        get
        {
            return _instance;
        }
    }
    #endregion

    // Use this for initialization
    void Start () {

	}

    void Awake()
    {
        _instance = this;
        agents = new List<AgentController>();
    }
	
	// Update is called once per frame
	void Update () {

        CleanUpAgentsList();
        CalculateFlockingForcesForAllAgents();
        CalculateVelocityForAllAgents();
        MoveAllAgents();
       
    }

    void CleanUpAgentsList()
    {
        agents.RemoveAll(agent => agent == null);
    }

    /// <summary>
    /// Calculates the flocking forces for all agents
    /// </summary>
    void CalculateFlockingForcesForAllAgents()
    {
        foreach(AgentController agent in agents)
        {
            List<AgentController> neighborhoodAgents = new List<AgentController>();
            foreach(AgentController neighborAgent in agents)
            {
                if(Vector3.Distance(agent.transform.position, neighborAgent.transform.position) <= neighborhoodRadius && neighborAgent != agent)
                {
                    neighborhoodAgents.Add(neighborAgent);
                }
            }
            agent.ComputeFlockingForces(neighborhoodAgents, allignmentWeight, cohesionWeight, seperationWeight);
        }
    }

    /// <summary>
    /// Calculate the velocity for all agents
    /// </summary>
    void CalculateVelocityForAllAgents()
    {
        foreach (AgentController agent in agents)
        {
            agent.CalculateVelocity();
        }
    }

    /// <summary>
    /// Applies the velocity to all agents and thereby moves them
    /// </summary>
    void MoveAllAgents()
    {
        foreach(AgentController agent in agents)
        {
            agent.ApplyVelocity();
        }
    }

    /// <summary>
    /// Add an agent to the flocking controller
    /// </summary>
    /// <param name="agent">The agentcontroller to be added</param>
    public void AddAgent(AgentController agent)
    {
        agents.Add(agent);
    }
}
