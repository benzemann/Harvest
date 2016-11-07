using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour {

    public float costs;
    public GameObject[] upgrades;
    public string textOnButton;
    public GameObject scaffold;
    GameObject scaffoldGO;
    public GameObject[] turretsToBeUnlocked;
    public float scaffoldUpTime;
    public float scaffoldDownTime;
    public float buildingBuildTime;
    public float buildingStartHeight;
    public float buildingEndHeight;
    public float scaffoldStartHeight;
    public float scaffoldEndHeight;
    float timeBuilding;
    public bool isReady = false;
	// Use this for initialization
	void Start () {
        int i = 0;
        foreach(GameObject upgrade in upgrades)
        {
            GameObject upgradeGo = Instantiate(upgrade, Vector3.zero, Quaternion.identity) as GameObject;
            upgrades[i] = upgradeGo;
            i++;
        }
        transform.position = new Vector3(transform.position.x, buildingStartHeight, transform.position.z);
        scaffoldGO = Instantiate(scaffold, new Vector3(transform.position.x, scaffoldStartHeight, transform.position.z), Quaternion.identity) as GameObject;
    }

    // Update is called once per frame
    void Update () {
        if (!isReady)
        {
            UpdateBuildAnimation();
        }
	}

    void UpdateBuildAnimation()
    {
        timeBuilding += Time.deltaTime;
        if (timeBuilding <= scaffoldUpTime)
        {
            transform.position = new Vector3(transform.position.x, buildingStartHeight, transform.position.z);
            scaffoldGO.transform.position = new Vector3(transform.position.x, (1.0f - (timeBuilding / scaffoldUpTime)) * scaffoldStartHeight + (timeBuilding / scaffoldUpTime) * scaffoldEndHeight, transform.position.z);
        }
        if (timeBuilding > scaffoldUpTime && timeBuilding <= (scaffoldUpTime + buildingBuildTime))
        {
            float percentage = (timeBuilding - scaffoldUpTime) / (buildingBuildTime);
            transform.position = new Vector3(transform.position.x, (1.0f - percentage) * buildingStartHeight + percentage * buildingEndHeight, transform.position.z);
        }
        if (timeBuilding <= (scaffoldUpTime + buildingBuildTime + scaffoldDownTime) && timeBuilding > scaffoldUpTime + buildingBuildTime)
        {
            float percentage = (timeBuilding - (scaffoldUpTime + buildingBuildTime)) / (scaffoldDownTime);
            scaffoldGO.transform.position = new Vector3(transform.position.x, (1.0f - percentage) * scaffoldEndHeight + percentage * scaffoldStartHeight, transform.position.z);
        }
        if (timeBuilding >= buildingBuildTime + scaffoldUpTime + scaffoldDownTime)
        {
            Destroy(scaffoldGO);
            isReady = true;
            GameObject player = GameObject.Find("Player");
            foreach (GameObject turret in turretsToBeUnlocked)
                player.GetComponent<Player>().turrets.Add(turret);
        }
    }

}
