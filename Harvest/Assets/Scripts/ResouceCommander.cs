using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Selectable)), RequireComponent(typeof(AgentController)), RequireComponent(typeof(ResourceCollector))]
public class ResouceCommander : MonoBehaviour {

	// Update is called once per frame
	void Update () {
        // Check for mouse down and if the gameobject is selected
        if (GetComponent<Selectable>().IsSelected && Input.GetMouseButtonUp(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.GetComponent<Resource>() != null)
                {
                    GetComponent<AgentController>().GoToPos(hit.transform.position);
                    GetComponent<ResourceCollector>().Target = hit.transform.GetComponent<Resource>();
                }
            }
        }
    }
}
