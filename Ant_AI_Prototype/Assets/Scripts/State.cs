using UnityEngine;
using System.Collections;

public enum StateID
{
    NullStateID = 0, // Non existing state
    IdleID = 1,
    ScoutID = 2
}

public abstract class State {

    #region protected
    protected StateID id;
    #endregion
    #region public
    public StateID Id { get { return id; } }
    #endregion

    /// <summary>
    /// Is called when entering the state.
    /// </summary>
    public virtual void Enter() { }

    /// <summary>
    /// Is called when changing from this state to another.
    /// </summary>
    public virtual void Exit() { }

    /// <summary>
    /// Is called every frame when this state is the current state.
    /// </summary>
    public virtual void Execute() { }

    /// <summary>
    /// Checks whether this state should be changed to another.
    /// </summary>
    public virtual void Reason() { }

}

///// WORKER ANT STATES /////

public class IdleStateWorker : State
{

    WorkerAnt ant;

    public IdleStateWorker(WorkerAnt a)
    {
        id = StateID.IdleID;
        ant = a;
    }

    public override void Enter()
    {
        Debug.Log("Enter idle");
    }

    public override void Exit()
    {
        Debug.Log("Exit idle");
    }

    public override void Reason()
    {
        GameObject enemy;
        GameObject resource;
        if(ant.IsEnemyClose(out enemy))
        {
            
        } else if (ant.IsResourcesClose(out resource))
        {

        }
        ant.ChangeState(StateID.ScoutID);
    }
}

public class ScoutStateWorker : State
{

    WorkerAnt ant;

    public ScoutStateWorker(WorkerAnt a)
    {
        id = StateID.ScoutID;
        ant = a;
    }

    public override void Enter()
    {
        Debug.Log("Enter scout");
    }

    public override void Exit()
    {
        Debug.Log("Exit scout");
    }

    public override void Reason()
    {
        
    }
}