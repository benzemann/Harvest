using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Selectable)), RequireComponent(typeof(AgentController))]
public class MoveCommander : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
        // Check for mouse down and if the gameobject is selected
        if (GetComponent<Selectable>().IsSelected && Input.GetMouseButtonUp(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    // Tell the agent to move to the position
                    GetComponent<AgentController>().GoToPos(hit.point);
                }
            }
        }
	}

}
