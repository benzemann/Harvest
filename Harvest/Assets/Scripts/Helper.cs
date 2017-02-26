using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : Singleton<Helper> {

	
    public GameObject GetClosestObject(Vector3 position, GameObject[] objects, float maxDistance = float.MaxValue)
    {
        var closestDistance = float.MaxValue;
        GameObject closestObject = null;
        for(int i = 0; i < objects.Length; i++)
        {
            var distance = Vector3.Distance(position, objects[i].transform.position);
            if(distance <= maxDistance && distance < closestDistance)
            {
                closestDistance = distance;
                closestObject = objects[i];
            }
        }
        return closestObject;
    }

}
