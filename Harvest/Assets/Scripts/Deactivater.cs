using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deactivater : MonoBehaviour {

    [SerializeField, Tooltip("The seconds until the object will be deactivated(or destroyed)")]
    private float secondsUntilDeactivate;
    [SerializeField, Tooltip("If the object should be destroyed instead of deactivated when the time is up.")]
    private bool destroy = false;

    private float timeAtActive;
    private bool hasBeenDeactivated;

    private void Start()
    {
        if (this.gameObject.activeInHierarchy)
        {
            timeAtActive = Time.time;
        }
    }

    // Update is called once per frame
    void Update () {
        if (hasBeenDeactivated)
        {
            hasBeenDeactivated = false;
            timeAtActive = Time.time;
        }

        if(Time.time - timeAtActive >= secondsUntilDeactivate)
        {
            if (destroy)
            {
                Destroy(this.gameObject);
            } else
            {
                hasBeenDeactivated = true;
                this.gameObject.SetActive(false);
            }
        }

	}
}
