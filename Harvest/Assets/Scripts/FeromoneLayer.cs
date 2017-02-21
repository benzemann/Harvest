using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeromoneLayer : MonoBehaviour {

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
        if(gridX != oldGridX || gridY != oldGridY)
        {
            FeromoneManager.Instance.AddFeromoneValue(gridX, gridY, 1f, oldGridX, oldGridY);
            oldGridX = gridX;
            oldGridY = gridY;
        }
    }
}
