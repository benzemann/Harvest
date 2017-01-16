using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : Singleton<AgentManager> {

    #region Private
    List<AgentController> agents;
    #endregion

    // Use this for initialization
    void Awake()
    {
        agents = new List<AgentController>();
    }

    // Update is called once per frame
    void Update()
    {

        // Update all agents
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
        foreach (AgentController agent in agents)
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
