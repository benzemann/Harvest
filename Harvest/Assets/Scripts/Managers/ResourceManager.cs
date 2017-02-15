using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>{

    private List<Resource> resources;

    public Resource[] AllResources { get { return resources.ToArray(); } }

    // Use this for initialization
    void Awake () {
        resources = new List<Resource>();
	}

	public void AddResource(Resource resource)
    {
        resources.Add(resource);
    }

    public void RemoveResource(Resource resource)
    {
        if (resources.Contains(resource))
            resources.Remove(resource);
    }
}
