using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AgentController)), RequireComponent(typeof(TargetFinder))]
public class WarriorAntAI : MonoBehaviour {

    FSM fsm;
    Animator animator;
    AgentController ac;
    TargetFinder targetFinder;

    public StateID currentState;
	// Use this for initialization
	void Start () {

        // Initialize the FSM and its states
        fsm = new FSM();

        WarriorAntIdle idle = new WarriorAntIdle(this.gameObject);
        WarriorAntScout scout = new WarriorAntScout(this.gameObject);
        WarriorAntPursuit pursuit = new WarriorAntPursuit(this.gameObject);
        WarriorAntAttack attack = new WarriorAntAttack(this.gameObject);
        WarriorAntHarvesting harvesting = new WarriorAntHarvesting(this.gameObject);
        WarriorAntReturnHome returnHome = new WarriorAntReturnHome(this.gameObject);

        fsm.AddState(idle);
        fsm.AddState(scout);
        fsm.AddState(pursuit);
        fsm.AddState(attack);
        fsm.AddState(harvesting);
        fsm.AddState(returnHome);

        // Cache components
        ac = GetComponent<AgentController>();
        if(GetComponent<Animator>() != null)
            animator = GetComponent<Animator>();
        targetFinder = GetComponent<TargetFinder>();

        // Add to ant manager
        ObjectManager.Instance.AddAnt(this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
        fsm.UpdateState();
        currentState = fsm.CurrentStateID;
        if (animator != null)
            animator.SetFloat("Speed", GetComponent<AgentController>().CurrentSpeed);
	}
}
