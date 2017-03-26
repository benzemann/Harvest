using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vision : MonoBehaviour {
    [SerializeField, Tooltip("The layers which this object cannot see through")]
    private LayerMask layers;
    [SerializeField]
    private float heightOffset;

    /// <summary>
    /// Checks if the object is visible from the owner of this component
    /// </summary>
    /// <param name="obj">The object which should be checked if visible</param>
    /// <returns>Whether or not the input object is visible</returns>
	public bool CanISeeIt(GameObject obj)
    {
        RaycastHit hit;
        var dir = (obj.transform.position - transform.position).normalized;
        var dis = Vector3.Distance(obj.transform.position, transform.position);
        Ray ray = new Ray(this.transform.position + Vector3.up * heightOffset, dir);
        if (!Physics.Raycast(ray, out hit, dis, layers))
        {
           return true;
        } 
        return false;
    }
}
