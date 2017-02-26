using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeromoneLayer : MonoBehaviour {

    [SerializeField, Tooltip("How much feromone this object will lay each time it enters a feromone node")]
    private float addFeromoneValue;
    [SerializeField, Tooltip("Determines if the object always should lay feromones. If false, some other script most use this component")]
    private bool alwaysLayFeromones;

    public bool LayFeromones { get; set; }

    private int gridX;
    private int gridY;
    private int oldGridX;
    private int oldGridY;

	// Use this for initialization
	void Start () {
        FeromoneManager.Instance.WorldToGridCoords(transform.position, out gridX, out gridY);
        oldGridX = gridX;
        oldGridY = gridY;
	}
	
	// Update is called once per frame
	void Update () {
        FeromoneManager.Instance.WorldToGridCoords(transform.position, out gridX, out gridY);
        if(alwaysLayFeromones == true || LayFeromones == true)
        {
            LayFermoneTrail();
        }
    }

    private void LayFermoneTrail()
    {
        if (gridX != oldGridX || gridY != oldGridY)
        {
            FeromoneManager.Instance.AddFeromoneValue(gridX, gridY, addFeromoneValue, oldGridX, oldGridY);
            oldGridX = gridX;
            oldGridY = gridY;
        }
    }
}
