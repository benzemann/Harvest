using UnityEngine;
using System.Collections;

public class AntUtils : MonoBehaviour {

    public GameObject ant;
    public GameObject warriorAnt;
    public GameObject mainHive;
    public GameObject ressource;
    public Camera camera;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown("a"))
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.name == "Ground")
                {
                    GameObject antInstance = Instantiate(ant, hit.point + new Vector3(0.0f, ant.transform.localScale.y + 1.0f, 0.0f), Quaternion.identity) as GameObject;
                    antInstance.GetComponent<Ant>().SetHive(mainHive);
                }
            }
        }
        if (Input.GetKeyDown("w"))
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.name == "Ground")
                {
                    GameObject antInstance = Instantiate(warriorAnt, hit.point + new Vector3(0.0f, warriorAnt.transform.localScale.y + 1.0f, 0.0f), Quaternion.identity) as GameObject;
                    antInstance.GetComponent<WarriorAnt>().SetHive(mainHive);
                }
            }
        }
        if (Input.GetKeyDown("r"))
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.name == "Ground")
                {
                    Instantiate(ressource, hit.point, Quaternion.identity);
                }
            }
        }

    }
}
