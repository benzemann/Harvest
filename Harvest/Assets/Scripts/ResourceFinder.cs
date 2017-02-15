using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceFinder : MonoBehaviour {

    [SerializeField, Tooltip("The radius of which the object will look for resources")]
    private float searchRadius;

    public bool GetClosestResource(out Resource closestResource)
    {
        Resource[] resources = ResourceManager.Instance.AllResources;

        if(resources.Length == 0)
        {
            closestResource = null;
            return false;
        }

        float closestDistance = float.MaxValue;
        int closestIndx = -1;
        for(int i = 0; i < resources.Length; i++)
        {
            var dis = Vector3.Distance(this.transform.position, resources[i].transform.position);
            if(dis <= searchRadius && dis < closestDistance)
            {
                closestDistance = dis;
                closestIndx = i;
            }
        }

        closestResource = resources[closestIndx != -1 ? closestIndx : 0];
        if (closestIndx == -1)
            return false;
        return true;
    }
}
