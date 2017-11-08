using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : Singleton<Manager> {
    [SerializeField]
    GameObject agentPrefab;
    [SerializeField]
    GameObject resPrefab;
    [SerializeField]
    Text text;

    float timeAtLastSpawn;
    float timeAtLastGeneration;
    List<GameObject> allRes;
    List<GameObject> allAgents;
    int currentGeneration = 0;
    int populationSize;
    int[] layers = new int[] { 4, 10, 10, 2 };
    List<NeuralNetwork> nets;

    // Parameters
    [SerializeField]
    int startAgents;
    [SerializeField]
    int startRes;
    [SerializeField]
    float resSpawnRate;
    [SerializeField]
    float generationTime;

	// Use this for initialization
	void Start () {
        // Init lists
        allRes = new List<GameObject>();
        allAgents = new List<GameObject>();
        nets = new List<NeuralNetwork>();
        // Create start res and agents
        for (int i = 0; i < startRes; i++)
            CreateRes();
        for (int i = 0; i < startAgents; i++)
            CreateAgent();

        timeAtLastGeneration = Time.time;
    }
	
	// Update is called once per frame
	void Update () {
        populationSize = allAgents.Count;
		// Spawn res
        if (Time.time - timeAtLastSpawn >= resSpawnRate)
        {
            CreateRes();
            timeAtLastSpawn = Time.time;
        }
        // New generation
        if(Time.time - timeAtLastGeneration >= generationTime)
        {
            NewGeneration();
            timeAtLastGeneration = Time.time;
        }

        UpdateDebugText();
	}

    void NewGeneration()
    {
        nets.Sort();
        for (int i = 0; i < populationSize / 2 ; i++)
        {
            nets[i] = new NeuralNetwork(nets[i + (populationSize / 2)], nets[i].Agent);
            nets[i].Mutate();
            nets[i + (populationSize / 2)] = new NeuralNetwork(nets[i + (populationSize / 2)], nets[i + (populationSize / 2)].Agent);
        }

        for (int i = 0; i < populationSize; i++)
        {
            nets[i].SetFitness(0f);
        }

        currentGeneration++;
    }

    void CreateAgent()
    {
        if (agentPrefab == null)
            return;
        var agent = Instantiate(agentPrefab, new Vector3(Random.Range(-49f, 49f), 0.0f, Random.Range(-49f, 49f)), Quaternion.identity) as GameObject;
        allAgents.Add(agent);
        var network = new NeuralNetwork(layers);
        agent.GetComponent<Agent>().Init(network);
        network.Agent = agent;
        nets.Add(network);
    }

    void CreateRes()
    {
        if (resPrefab == null || allRes.Count >= 100)
            return;
        allRes.Add(Instantiate(resPrefab, new Vector3(Random.Range(-49f, 49f), 0.25f, Random.Range(-49f, 49f)), Quaternion.identity) as GameObject);
    }

    public float DistanceToClosestRes(Vector3 pos)
    {
        float closestDis = float.MaxValue;
        for(int i = 0; i < allRes.Count; i++)
        {
            var dis = Vector3.Distance(pos, allRes[i].transform.position);
            if(dis < closestDis)
            {
                closestDis = dis;
            }
        }
        return closestDis;
    }

    public Vector3 PosOfClosestRes(Vector3 pos)
    {
        float closestDis = float.MaxValue;
        Vector3 closestPos = closestPos = Vector3.zero;
        for (int i = 0; i < allRes.Count; i++)
        {
            var dis = Vector3.Distance(pos, allRes[i].transform.position);
            if (dis < closestDis)
            {
                closestDis = dis;
                closestPos = allRes[i].transform.position;
            }
        }
        return closestPos;
    }

    public void DeleteRes(GameObject res)
    {
        if (allRes.Contains(res))
        {
            allRes.Remove(res);
            Destroy(res);
        }
    }

    void UpdateDebugText()
    {
        var t = "Neural network info:" + System.Environment.NewLine;
        t += "Generation: " + currentGeneration + System.Environment.NewLine;
        t += "Population: " + populationSize + System.Environment.NewLine;
        t += "Time until next generation: " + (generationTime - (Time.time - timeAtLastGeneration)) + System.Environment.NewLine;
        t += "Resources: " + allRes.Count;
        text.text = t;
    }
}
