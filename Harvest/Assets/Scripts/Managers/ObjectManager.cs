using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectManager : Singleton<ObjectManager> {

    private List<GameObject> ants;
    private List<GameObject> playerObjects;
    private List<GameObject> eggs;

    public GameObject[] AllPlayerObjects { get { return playerObjects.ToArray(); } }
    public GameObject[] AllAnts { get { return ants.ToArray(); } }
    public GameObject[] WarriorAnts {
        get
        {
            return (from ant in ants
                    where ant.GetComponent<WarriorAntAI>() != null
                    select ant).ToArray();
        }
    }
    public GameObject[] AllEggs
    {
        get { return eggs.ToArray(); }
    }
    public GameObject[] AllAvailableEggs
    {
        get {
            return (from egg in eggs
                    where egg.GetComponent<ResourceStorage>().RemainingStorageSpace > 0
                    select egg).ToArray(); }
    }

    private void Awake()
    {
        ants = new List<GameObject>();
        playerObjects = new List<GameObject>();
        eggs = new List<GameObject>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Add an ant to the manager
    /// </summary>
    /// <param name="ant">The ant to be added</param>
    public void AddAnt(GameObject ant)
    {
        ants.Add(ant);
    }

    /// <summary>
    /// Remove an ant from the manager
    /// </summary>
    /// <param name="ant"></param>
    public void RemoveAnt(GameObject ant)
    {
        if (ants.Contains(ant))
            ants.Remove(ant);
    }
    

    public void AddPlayerObject(GameObject obj)
    {
        playerObjects.Add(obj);
    }

    public void RemovePlayerObject(GameObject obj)
    {
        if(playerObjects.Contains(obj))
            playerObjects.Remove(obj);
    }

    public void AddEgg(GameObject egg)
    {
        eggs.Add(egg);
    }

    public void RemoveEgg(GameObject egg)
    {
        if (eggs.Contains(egg))
            eggs.Remove(egg);
    }
}
