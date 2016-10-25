using UnityEngine;
using System.Collections;

public class Refinery : MonoBehaviour {

    public float maxHealth;
    public float harvesterRespawnTime;
    float timeSinceLostHarvester;
    bool spawningNewHarvester = false;
    public float health;
    GameObject harvester;
    public GameObject harvesterPrefab;
    public float repairCosts;
    public float repairPrSec;
    public GameObject repairPrefab;
    public Vector3 repairLocalPos;
    GameObject repairGO;
    float timeSinceLastRepair;
    // Use this for initialization
    void Start () {
        harvester = GameObject.Find("Harvester");
        health = maxHealth;
	}
	
	// Update is called once per frame
	void Update () {
	    if(harvester == null && !spawningNewHarvester)
        {
            timeSinceLostHarvester = Time.time;
            spawningNewHarvester = true;
        }
        if (spawningNewHarvester)
        {
            if(Time.time - timeSinceLostHarvester > harvesterRespawnTime)
            {
                SpawnHarvester();
            }
        }
        if (health < maxHealth)
        {
            timeSinceLastRepair += Time.deltaTime;
            if (timeSinceLastRepair >= 1.0f)
            {
                if (GameObject.Find("Player").GetComponent<Player>().Pay(repairCosts))
                {
                    if (repairGO == null)
                        InstatiateRepairGO();
                    health += repairPrSec;
                    if (health > maxHealth)
                    {
                        health = maxHealth;
                    }
                    timeSinceLastRepair = 0f;
                }
                else
                {
                    RemoveRepairGO();
                }
            }
        }
        else
        {
            RemoveRepairGO();
        }
    }

    void SpawnHarvester()
    {
        GameObject h = Instantiate(harvesterPrefab, transform.position, Quaternion.identity) as GameObject;
        h.name = "Harvester";
        harvester = h;
        harvester.GetComponent<Harvester>().ressources = 0f;
        GameObject[] upgrades = GameObject.FindGameObjectsWithTag("Upgrade");
        foreach (GameObject upgrade in upgrades)
        {
            upgrade.GetComponent<Upgrade>().ApplyUpradeToThis(h);
        }
        spawningNewHarvester = false;
    }

    public void Damage(float d)
    {
        health -= d;
        if (health <= 0f)
            Destroy(this.gameObject);
    }

    public string[] InfoText()
    {
        string[] infoText = new string[2];
        infoText[0] = "Refinery";
        infoText[1] = "Healht : " + health + "/" + maxHealth;
        return infoText;
    }

    public void Repair(float amount)
    {
        health += amount;
        if(health > maxHealth)
        {
            health = maxHealth;
        }
    }

    void InstatiateRepairGO()
    {
        if (repairPrefab != null && repairGO == null)
        {
            repairGO = Instantiate(repairPrefab, repairLocalPos, Quaternion.identity) as GameObject;
            repairGO.transform.parent = this.transform;
            repairGO.transform.localPosition = repairLocalPos;
        }
    }

    void RemoveRepairGO()
    {
        if (repairGO != null)
            Destroy(repairGO);
    }
}
