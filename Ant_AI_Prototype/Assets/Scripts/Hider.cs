using UnityEngine;
using System.Collections;

public class Hider : MonoBehaviour {
	
    void Start()
    {
        CheckIfVisible();
    }

	// Update is called once per frame
	void Update () {
        CheckIfVisible();
	}

    void CheckIfVisible()
    {
        if (FogOfWar.Instance == null)
            return;
        if (FogOfWar.Instance.IsVisible(transform.position))
        {
            GetComponent<Renderer>().enabled = true;
        }
        else
        {
            GetComponent<Renderer>().enabled = false;
        }
    }
}
