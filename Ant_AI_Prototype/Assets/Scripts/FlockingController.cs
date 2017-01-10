using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlockingController : MonoBehaviour {

    #region Private
    List<AgentController> agents;
    private static FlockingController _instance;
    #endregion

    #region Public
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
        _instance = this;
        agents = new List<AgentController>();
	}

    void Awake()
    {
        
    }
	
	// Update is called once per frame
	void Update () {

        CalculateVelocityForAllAgents();
        MoveAllAgents();

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
