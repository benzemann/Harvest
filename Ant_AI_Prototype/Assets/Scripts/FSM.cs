using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FSM {

    #region private
    private List<State> states;
    private StateID currentStateID;
    private State currentState;
    #endregion

    #region public 
    public StateID CurrentStateID { get { return currentStateID; } }
    public State CurrentState {  get { return currentState; } }
    #endregion

    public FSM()
    {
        states = new List<State>();
    }

    /// <summary>
    /// Updates the current state, should be called each frame.
    /// </summary>
    public void UpdateState()
    {
        if (currentStateID != StateID.NullStateID)
        {
            currentState.Execute();
            currentState.Reason();
        }
            
    }

    /// <summary>
    /// Add a state to the FSM.
    /// </summary>
    /// <param name="state">State to be added</param>
    public void AddState(State state)
    {
        if (state == null)
        {
            Debug.LogError("state added is null");
            return;
        }

        // If first added enter the state
        if(states.Count == 0)
        {
            states.Add(state);
            currentState = state;
            currentStateID = state.Id;
            currentState.Enter();
            return;
        }

        foreach(var s in states)
        {
            if(s.Id == state.Id)
            {
                Debug.LogError("Trying to add a state that is already in the FSM");
                return;
            }
        }

        states.Add(state);
    }

    /// <summary>
    /// Remove a state from the FSM.
    /// </summary>
    /// <param name="stateID">State ID of the state to be removed</param>
    public void DeleteState(StateID stateID)
    {
        foreach(var state in states)
        {
            if (state.Id == stateID)
            {
                states.Remove(state);
                return;
            }
        }
        Debug.LogError("Trying to delete state that is not in the FSM");
    }

    /// <summary>
    /// Exit the current state and enter the input state.
    /// </summary>
    /// <param name="stateID">The ID of the state it should change to</param>
    public void ChangeState(StateID stateID)
    {
        if(stateID == StateID.NullStateID)
        {
            Debug.LogError("Cannot change state to null state");
            return;
        }

        foreach(var state in states) {
            if(state.Id == stateID)
            {
                // Perform transition from currentState to input state
                currentState.Exit();
                currentState = state;
                currentStateID = state.Id;
                currentState.Enter();
                return;
            }
        }
        Debug.LogError("Trying to change state to an unknown state to the FSM");
    }
    
}
