using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Selectable)), RequireComponent(typeof(AgentController))]
public class MoveCommander : MonoBehaviour {

    private void Start()
    {
    }

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
                    GameObject clickObject = ObjectPoolsManager.Instance.groundClickPool.GetPooledObject();
                    if (clickObject != null)
                    {
                        clickObject.transform.position = hit.point;
                        clickObject.transform.rotation = Quaternion.identity;
                        clickObject.SetActive(true);
                    }
                    // Make sure resource collector disables its target
                    if (GetComponent<ResourceCollector>() != null && GetComponent<ResourceCollector>().Target != null)
                        GetComponent<ResourceCollector>().Target = null;
                }
            }
        }
	}

}
