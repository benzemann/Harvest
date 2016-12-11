using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AgentController))]
public abstract class AntNew : MonoBehaviour {

    #region protected
    protected FSM fsm;
    protected AgentController agentController;
    protected Animator anim;
    [Header("Stats")]
    [SerializeField]
    protected float maxHealth;
    protected float health;
    [SerializeField]
    protected float speed;
    [SerializeField]
    [Tooltip("The mass of the ant. If mass is a high number the ant will turn slowly.")]
    protected float mass;
    [SerializeField]
    protected float lookRadius;
    #endregion
    #region private

    #endregion

    /// <summary>
    /// Should be called in the Start function
    /// </summary>
    public void Init()
    {
        fsm = new FSM();
        // Set current health
        health = maxHealth;
        // Set the agencontroller variables
        agentController = GetComponent<AgentController>();
        agentController.Speed = speed;
        agentController.Mass = mass;
        // Animator
        anim = GetComponent<Animator>();
    }

    /// <summary>
    /// Updates the FSM system, should be called once per frame.
    /// </summary>
    public void UpdateFSM()
    {
        fsm.UpdateState();
    }

    /// <summary>
    /// Returns true if enemy is close. Optional the out gameobject is the closest enemy if any.
    /// </summary>
    /// <param name="enemyPos">Position of closest enemy</param>
    /// <returns></returns>
    public bool IsEnemyClose(out GameObject closeEnemy)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Player");
        closeEnemy = null;
        float closestDis = float.MaxValue;
        bool isEnemyClose = false;
        foreach(var enemy in enemies)
        {
            float dis = Vector3.Distance(this.transform.position, enemy.transform.position);
            if (dis <= lookRadius)
            {
                isEnemyClose = true;
                if (dis < closestDis)
                    closeEnemy = enemy;
            }
        }
        return isEnemyClose;
    }

    /// <summary>
    /// Cange the current state.
    /// </summary>
    /// <param name="newStateID">The state ID of the new state</param>
    public void ChangeState(StateID newStateID)
    {
        fsm.ChangeState(newStateID);
    }
}
