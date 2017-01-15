using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignToGround : MonoBehaviour {
    public LayerMask lm;
    void Update()
    {
        //make platform adjust terrain rotation
        RaycastHit hit;
        //Make raycast direction down
        Vector3 dir = transform.TransformDirection(Vector3.down);
        
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 10, lm))
        {
            //this is for getting distance from object to the ground
            var GroundDis = hit.distance;
            //with this you rotate object to adjust with terrain
            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            transform.position = hit.point;
            
        }
    }
}
