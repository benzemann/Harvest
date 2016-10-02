using UnityEngine;
using System.Collections;

public class DistressBeacon : MonoBehaviour {

    public int maxDistressLevel;
    public int startDistressLevel;
    public int increasAmount;
    public float maxDistressTime;

    int distressLevel;
    float timeSinceStart;
    GameObject hive;
	// Use this for initialization
	void Start () {
        distressLevel = startDistressLevel;
	}
	
	// Update is called once per frame
	void Update () {
        
        timeSinceStart += Time.deltaTime;
        if (timeSinceStart >= maxDistressTime)
            hive.GetComponent<Hive>().RemoveDistressBeacon(this.gameObject);
        //if (!LookForEnemies())
          //  hive.GetComponent<Hive>().RemoveDistressBeacon(this.gameObject);
	}

    public void SetHive(GameObject h)
    {
        hive = h;
    }

    public int GetDistressLevel()
    {
        return distressLevel;
    }

    public void IncrementDistressLevel()
    {
        distressLevel += increasAmount;
        if(distressLevel > maxDistressLevel)
        {
            hive.GetComponent<Hive>().RemoveDistressBeacon(this.gameObject);
        }
    }

    bool LookForEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject enemy in enemies)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) <= 5.0f )
            {
                return true;
            }
        }
        return false;
    }
}
