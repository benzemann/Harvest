using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceReciever : MonoBehaviour {

	public void Store(int amount, out int remainder)
    {
        remainder = 0;
        if(GetComponent<ResourceStorage>() != null)
        {
            GetComponent<ResourceStorage>().StoreResource(amount, out remainder);
        }
    }

}
