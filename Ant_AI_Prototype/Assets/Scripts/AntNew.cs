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
    [Tooltip("Maximum health of the ant.")]
    protected float maxHealth;
    protected float health;
    [SerializeField]
    protected float speed;
    [SerializeField]
    [Tooltip("The mass of the ant. If mass is a high number the ant will turn slowly.")]
    protected float mass;
    [SerializeField]
    [Tooltip("The radius which the ant will see enemies")]
    protected float lookRadius;
    #endregion
    #region private
    private Hive hive;
    #endregion
    #region public 
    public Hive Hive { get { return hive; } set { hive = value; } }
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
    /// Commands the ant to go to a specific position.
    /// </summary>
    /// <param name="pos"></param>
    public void GoToPos(Vector3 pos)
    {
        agentController.GoToPos(pos);
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

    /// <summary>
    /// Tells the ant to go to its hive.
    /// </summary>
    public void ReturnHome()
    {
        if (hive == null) return;
        agentController.GoToPos(hive.transform.position);
    }

    /// <summary>
    /// The ant will enter its own hive.
    /// </summary>
    public void EnterHive()
    {

    }
}
