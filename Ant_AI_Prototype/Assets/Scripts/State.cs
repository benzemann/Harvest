using UnityEngine;
using System.Collections;

public enum StateID
{
    NullStateID = 0, // Non existing state
    IdleID = 1,
    ScoutID = 2,
    GoingHomeID = 3
}

public abstract class State {

    #region private
    private float startTime;
    #endregion
    #region protected
    protected StateID id;
    #endregion
    #region public
    public StateID Id { get { return id; } }
    #endregion

    /// <summary>
    /// Is called when entering the state.
    /// </summary>
    public virtual void Enter() { startTime = Time.time; }

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

    public float GetTimeInState()
    {
        return Time.time - startTime;
    }

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
        base.Enter();
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
    Vector3 targetPos;

    public ScoutStateWorker(WorkerAnt a)
    {
        id = StateID.ScoutID;
        ant = a;
    }

    public override void Enter()
    {
        Debug.Log("Enter scout");
        base.Enter();
        Vector3 ranPos = ant.GetRandomNearLocation();
        targetPos = ranPos;
        ant.GoToPos(ranPos);
    }

    public override void Exit()
    {
        Debug.Log("Exit scout");
    }

    public override void Reason()
    {
        // Check if any Enemies are near
        GameObject enemy;
        GameObject resource;
        if (ant.IsEnemyClose(out enemy))
        {
            ant.ChangeState(StateID.GoingHomeID);
        }
        // Check if any resources are near
        if (ant.IsResourcesClose(out resource))
        {

        }
        // Check if has reached target, if so change to IDLE state
        if (Vector3.Distance(ant.gameObject.transform.position, targetPos) < 1f)
        {
            ant.ChangeState(StateID.IdleID);
        }
        // Check if max scout time has been reached
        if(GetTimeInState() >= ant.MaxScoutTime)
        {
            ant.ChangeState(StateID.GoingHomeID);
        }
    }
}

public class GoingHomeWorker : State
{
    WorkerAnt ant;
    
    public GoingHomeWorker(WorkerAnt a)
    {
        id = StateID.GoingHomeID;
        ant = a;
    }

    public override void Enter()
    {
        Debug.Log("Enter going home");
        base.Enter();
        ant.ReturnHome();
    }

    public override void Exit()
    {
        Debug.Log("Exit going home");
    }

    public override void Reason()
    {
        // Check if ant has a hive, if not go to idle state
        if (ant.Hive == null)
        {
            ant.ChangeState(StateID.IdleID);
            return;
        }
        // Check if distance to hive entrance is close enough to enter
        if(Vector3.Distance(ant.transform.position, ant.Hive.transform.position) < 0.5f){
            ant.EnterHive();
        }
    }
}