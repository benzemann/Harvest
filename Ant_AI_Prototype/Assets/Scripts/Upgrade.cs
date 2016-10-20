using UnityEngine;
using System.Collections;

public class Upgrade : MonoBehaviour {

    public float costs;
    public enum UpgradeType { Harvester, Turret, Phasma, Fire, Missile };
    public enum Upgrades { health, healthRegen, Damage, HarvestTime, UnloadTime, RessourceCapacity, Speed }
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
        if (!hasBeenApplied)
            return;
        switch (upgradeType)
        {
            case UpgradeType.Harvester:
                if (g.GetComponent<Harvester>() == null)
                    return;
                switch (upgrade)
                {
                    case Upgrades.health:
                        g.GetComponent<Harvester>().maxHealth += upgradeAmount;
                        g.GetComponent<Harvester>().AddHealth(upgradeAmount);
                        break;
                    case Upgrades.healthRegen:
                        g.GetComponent<Harvester>().repairPrSec += upgradeAmount;
                        break;
                    case Upgrades.RessourceCapacity:
                        g.GetComponent<Harvester>().ressourceCapacity += upgradeAmount;
                        break;
                    case Upgrades.HarvestTime:
                        g.GetComponent<Harvester>().harvestTime += upgradeAmount;
                        break;
                    case Upgrades.UnloadTime:
                        g.GetComponent<Harvester>().unloadTime += upgradeAmount;
                        break;
                    case Upgrades.Speed:
                        g.GetComponent<Harvester>().AddSpeed(upgradeAmount);
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
            case UpgradeType.Phasma:
                if (g.GetComponent<Turret>() == null)
                    return;
                switch (upgrade)
                {
                    case Upgrades.health:

                        if (g.GetComponent<Turret>() != null)
                        {
                            if (g.GetComponent<Turret>().turretType != Turret.TurretType.Phasma)
                                return;
                            g.GetComponent<Turret>().maxHealth += upgradeAmount;
                            g.GetComponent<Turret>().Repair(upgradeAmount);
                        }

                        break;
                    case Upgrades.healthRegen:

                        if (g.GetComponent<Turret>() != null)
                        {
                            if (g.GetComponent<Turret>().turretType != Turret.TurretType.Phasma)
                                return;
                            g.GetComponent<Turret>().repairAmount += upgradeAmount;
                        }

                        break;
                    case Upgrades.Damage:
                        if (g.GetComponent<Turret>() != null)
                        {
                            if (g.GetComponent<Turret>().turretType != Turret.TurretType.Phasma)
                                return;
                            g.GetComponent<Turret>().damage += upgradeAmount;
                        }

                        break;
                    default:
                        Debug.LogError("Wrong upgrade to this upgrade type!");
                        break;
                }
                break;
            case UpgradeType.Fire:
                if (g.GetComponent<Turret>() == null)
                    return;
                switch (upgrade)
                {
                    case Upgrades.health:

                        if (g.GetComponent<Turret>() != null)
                        {
                            if (g.GetComponent<Turret>().turretType != Turret.TurretType.Fire)
                                return;
                            g.GetComponent<Turret>().maxHealth += upgradeAmount;
                            g.GetComponent<Turret>().Repair(upgradeAmount);
                        }

                        break;
                    case Upgrades.healthRegen:

                        if (g.GetComponent<Turret>() != null)
                        {
                            if (g.GetComponent<Turret>().turretType != Turret.TurretType.Fire)
                                return;
                            g.GetComponent<Turret>().repairAmount += upgradeAmount;
                        }

                        break;
                    case Upgrades.Damage:
                        if (g.GetComponent<Turret>() != null)
                        {
                            if (g.GetComponent<Turret>().turretType != Turret.TurretType.Fire)
                                return;
                            g.GetComponent<Turret>().damage += upgradeAmount;
                        }

                        break;
                    default:
                        Debug.LogError("Wrong upgrade to this upgrade type!");
                        break;
                }
                break;
            case UpgradeType.Missile:
                if (g.GetComponent<Turret>() == null)
                    return;
                switch (upgrade)
                {
                    case Upgrades.health:
                        if (g.GetComponent<Turret>() != null)
                        {
                            if (g.GetComponent<Turret>().turretType != Turret.TurretType.Missile)
                                return;
                            g.GetComponent<Turret>().maxHealth += upgradeAmount;
                            g.GetComponent<Turret>().Repair(upgradeAmount);
                        }

                        break;
                    case Upgrades.healthRegen:

                        if (g.GetComponent<Turret>() != null)
                        {
                            if (g.GetComponent<Turret>().turretType != Turret.TurretType.Missile)
                                return;
                            g.GetComponent<Turret>().repairAmount += upgradeAmount;
                        }

                        break;
                    case Upgrades.Damage:
                        if (g.GetComponent<Turret>() != null)
                        {
                            if (g.GetComponent<Turret>().turretType != Turret.TurretType.Missile)
                                return;
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
	
	// Update is called once per frame
	void Update () {
	
	}
}
