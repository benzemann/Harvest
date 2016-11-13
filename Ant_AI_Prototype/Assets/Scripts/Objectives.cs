using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Objectives : MonoBehaviour {

    public enum ObjectiveType { HarvestAmount, HoldRessources, BuildDrills }
    public ObjectiveType objectiveType;
    public float amount;
    float remainingAmount;
    public bool isComplete;

	// Use this for initialization
	void Start () {
        remainingAmount = amount;
        UpdateObjective(0f);

    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void UpdateObjective(float a)
    {
        if(objectiveType == ObjectiveType.HarvestAmount) {
            remainingAmount -= a;
            if (remainingAmount <= 0)
            {
                remainingAmount = 0;
                isComplete = true;
                GameObject.Find("Player").GetComponent<Player>().CheckVictory();
            }
        }
        else if(objectiveType == ObjectiveType.HoldRessources)
        {
            remainingAmount = GameObject.Find("Player").GetComponent<Player>().GetRessources();
            if(remainingAmount >= amount)
            {
                isComplete = true;
                GameObject.Find("Player").GetComponent<Player>().CheckVictory();
            }
        } else if (objectiveType == ObjectiveType.BuildDrills)
        {
            GameObject[] playerGOs = GameObject.FindGameObjectsWithTag("Player");
            int numberOfDrills = 0;
            foreach(GameObject playerGO in playerGOs)
            {
                if(playerGO.GetComponent<Turret>() != null)
                {
                    if(playerGO.GetComponent<Turret>().turretType == Turret.TurretType.Drill &&
                        playerGO.GetComponent<Turret>().IsReady())
                    {
                        numberOfDrills++;
                    }
                }
            }
            remainingAmount = amount - numberOfDrills;
            if(remainingAmount <= 0)
            {
                isComplete = true;
                GameObject.Find("Player").GetComponent<Player>().CheckVictory();
            }

        }

    }

    public string GetText()
    {
        switch (objectiveType)
        {
            case ObjectiveType.HarvestAmount:
                if (remainingAmount > 0)
                    return "- Harvest " + amount + " ressources. Remaining: " + remainingAmount;
                else
                    return "- Harvest " + amount + " ressources. Completed";
            case ObjectiveType.HoldRessources:
                if (!isComplete)
                    return "- Hold " + amount + " resources. You hold " + remainingAmount + "/" + amount;
                else
                    return "- Hold " + amount + " resources. Complete";
            case ObjectiveType.BuildDrills:
                if (!isComplete)
                    return "- Build " + amount + " drills. Remaining: " + remainingAmount;
                else
                    return "- Build " + amount + " drills. Complete";
        } 
        return "";
    }
}
