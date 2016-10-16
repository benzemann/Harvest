using UnityEngine;
using System.Collections;

public class Upgrade : MonoBehaviour {

    public float costs;
    public enum UpgradeType { Harvester, Turret };
    public enum Upgrades { health, healthRegen, Damage }
    public UpgradeType upgradeType;
    public Upgrades upgrade;
    public float upgradeAmount;
    public string textOnButton;
    public bool hasBeenApplied = false;
	// Use this for initialization
	void Start () {
        
	}
    public void ApplyUpradeToThis(GameObject g)
    {
        //if (!hasBeenApplied)
        //return;
        Debug.Log(upgradeType);
        switch (upgradeType)
        {
            case UpgradeType.Harvester:
                if (g.GetComponent<Harvester>() == null)
                    return;
                switch (upgrade)
                {
                    case Upgrades.health:
                        if (g != null)
                        {
                            g.GetComponent<Harvester>().maxHealth += upgradeAmount;
                            g.GetComponent<Harvester>().AddHealth(upgradeAmount);
                        }
                        break;
                    case Upgrades.healthRegen:
                        if (g != null)
                        {
                            g.GetComponent<Harvester>().repairPrSec += upgradeAmount;
                        }
                        break;
                    default:
                        Debug.LogError("Wrong upgrade to this upgrade type!");
                        break;
                }
                break;
            case UpgradeType.Turret:
                if (g.GetComponent<Turret>() == null)
                    return;
                switch (upgrade)
                {
                    case Upgrades.health:
                        
                        if (g.GetComponent<Turret>() != null)
                        {
                            g.GetComponent<Turret>().maxHealth += upgradeAmount;
                            g.GetComponent<Turret>().Repair(upgradeAmount);
                        }
                        
                        break;
                    case Upgrades.healthRegen:
                        
                        if (g.GetComponent<Turret>() != null)
                        {
                            g.GetComponent<Turret>().repairAmount += upgradeAmount;
                        }
                        
                        break;
                    case Upgrades.Damage:
                        if (g.GetComponent<Turret>() != null)
                        {
                            g.GetComponent<Turret>().damage += upgradeAmount;
                        }
                        
                        break;
                    default:
                        Debug.LogError("Wrong upgrade to this upgrade type!");
                        break;
                }
                break;
        }
    }
    public void ApplyUpgrade()
    {
        hasBeenApplied = true;
        switch (upgradeType)
        {
            case UpgradeType.Harvester:
                GameObject harvester = GameObject.Find("Harvester");
                switch (upgrade)
                {
                    case Upgrades.health:
                        if(harvester != null)
                        {
                            harvester.GetComponent<Harvester>().maxHealth += upgradeAmount;
                            harvester.GetComponent<Harvester>().AddHealth(upgradeAmount);
                        }
                        break;
                    case Upgrades.healthRegen:
                        if (harvester != null)
                        {
                            harvester.GetComponent<Harvester>().repairPrSec += upgradeAmount;
                        }
                        break;
                    default:
                        Debug.LogError("Wrong upgrade to this upgrade type!");
                        break;
                }
                break;
            case UpgradeType.Turret:
                GameObject[] turrets = GameObject.FindGameObjectsWithTag("Player");
                switch (upgrade)
                {
                    case Upgrades.health:
                        foreach(GameObject turret in turrets)
                        {
                            if(turret.GetComponent<Turret>() != null)
                            {
                                turret.GetComponent<Turret>().maxHealth += upgradeAmount;
                                turret.GetComponent<Turret>().Repair(upgradeAmount);
                            }
                        }
                        break;
                    case Upgrades.healthRegen:
                        foreach (GameObject turret in turrets)
                        {
                            if (turret.GetComponent<Turret>() != null)
                            {
                                turret.GetComponent<Turret>().repairAmount += upgradeAmount;
                            }
                        }
                        break;
                    case Upgrades.Damage:
                        foreach (GameObject turret in turrets)
                        {
                            if (turret.GetComponent<Turret>() != null)
                            {
                                turret.GetComponent<Turret>().damage += upgradeAmount;
                            }
                        }
                        break;
                    default:
                        Debug.LogError("Wrong upgrade to this upgrade type!");
                        break;
                }
                break;
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
