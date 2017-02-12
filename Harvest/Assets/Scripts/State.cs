using UnityEngine;
using System.Collections;

public enum StateID
{
    NullStateID = 0, // Non existing state
    IdleID = 1,
    ScoutID = 2,
    PursuitID = 3,
    AttackID = 4
}

public abstract class State
{

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
    public virtual StateID Reason() { return Id; }

}
