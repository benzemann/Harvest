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
	}

    void SpawnHarvester()
    {
        GameObject h = Instantiate(harvesterPrefab, transform.position, Quaternion.identity) as GameObject;
        h.name = "Harvester";
        harvester = h;
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
}
