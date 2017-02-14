using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        ApplyPushingForces();
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

    void ApplyPushingForces()
    {
        for(int i = 0; i < agents.Count; i++)
        {
            if (!agents[i].ApplyPushingForces)
                continue;
            List<AgentController> neighborhood = new List<AgentController>();
            for(int j = 0; j < agents.Count; j++)
            {
                if (i == j || !agents[j].ApplyPushingForces)
                    continue;

                if(Vector3.Distance(agents[i].transform.position,
                    agents[j].transform.position) <= agents[i].PushRadius)
                {
                    neighborhood.Add(agents[j]);
                }
            }
            agents[i].ComputeFlockingForces(neighborhood, 0.0f, 0.0f, 0.0f);
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

    /// <summary>
    /// Remove all null references from the agent list
    /// </summary>
    private void CleanAgentList()
    {
        agents.RemoveAll(agent => agent == null);
    }

    public void RemoveAgent(AgentController agent)
    {
        if (agents.Contains(agent))
            agents.Remove(agent);
    }

}
