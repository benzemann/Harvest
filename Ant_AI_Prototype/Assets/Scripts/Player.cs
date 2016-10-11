using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour {

    public float plateCost;
    public float phasmaTurretCosts;
    public float flameTurretCosts;
    public float sniperTurretCosts;
    public float startRessources;
    GameObject selection;
    public Material selectionMat;
    Camera camera;
    GameObject selectionPlane;
    public Text infoText;
    public Text ressourcesText;
    public GameObject buildButtons;
    public GameObject repairButtons;
    public GameObject phasmaTurret;
    public GameObject flameTurret;
    public GameObject sniperTurret;
    float ressources;

	// Use this for initialization
	void Start () {
        ressources = startRessources;
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
        if(selection == null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    GameObject parentHit = hit.transform.gameObject;
                    while(parentHit.transform.parent != null)
                    {
                        parentHit = parentHit.transform.parent.gameObject;
                    }
                    if (parentHit.tag == "Player" ||
                        parentHit.name == "Hive" ||
                        parentHit.tag == "TurretSpot" ||
                        parentHit.tag == "Ants")
                    {
                        //CancleRepair();
                        CancleBuild();
                        selection = parentHit.transform.gameObject;
                        if (selection.tag == "TurretSpot")
                            ShowBuildButtons();
                        CreateSelectionPlane();
                        //ShowRepairButtons(); // Will only show if turret!
                    } 
                }
            }
        } else
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    GameObject parentHit = hit.transform.gameObject;
                    while (parentHit.transform.parent != null)
                    {
                        parentHit = parentHit.transform.parent.gameObject;
                    }
                    if (parentHit.tag == "Player" ||
                        parentHit.name == "Hive" || 
                        parentHit.tag == "TurretSpot" ||
                        parentHit.tag == "Ants")
                    {
                        //CancleRepair();
                        CancleBuild();
                        selection = parentHit;
                        if (selection.tag == "TurretSpot")
                            ShowBuildButtons();
                        CreateSelectionPlane();
                        //ShowRepairButtons(); // Will only show if turret or refinery!
                        return;
                    } else if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        selection = null;
                        //CancleRepair();
                        CancleBuild();
                    }
                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                if (selection == null)
                    return;
                if (selection.GetComponent<Harvester>() == null)
                    return;

                RaycastHit hit;
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.gameObject.name == "Ground")
                    {
                        selection.GetComponent<Harvester>().GoToPosition(hit.point);
                    } else if(hit.transform.gameObject.tag == "Ressources")
                    {
                        selection.GetComponent<Harvester>().GoToRessource(hit.transform.gameObject);
                    } else if(hit.transform.gameObject.name == "Refinery" || hit.transform.gameObject.name == "RefineryUnload")
                    {
                        selection.GetComponent<Harvester>().GoHome();
                    } else if(hit.transform.gameObject.tag == "TurretSpot")
                    {
                        if (!hit.transform.gameObject.GetComponent<TurretSpot>().HasPlate())
                        {
                            selection.GetComponent<Harvester>().GoPlacePlate(hit.transform.gameObject);
                        }
                    } 
                }
                
            }
        }
        UpdateSelectionPlane();
        UpdateSelectionText();
        ressourcesText.text = "Ressources : " + ressources;
    }

    void ShowBuildButtons()
    {
        if(selection.GetComponent<TurretSpot>().HasPlate())
            buildButtons.SetActive(true);
    }

    void ShowRepairButtons()
    {
        if(selection.GetComponent<Turret>() != null || selection.GetComponent<Refinery>() != null)
        {
            repairButtons.SetActive(true);
        }
    }

    void CreateSelectionPlane()
    {
        if (selection == null)
            return;
        if (selectionPlane != null)
        {
            Destroy(selectionPlane);
        }
        selectionPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        float size = selection.GetComponent<Renderer>().bounds.extents.magnitude * .25f;
        selectionPlane.transform.localScale = new Vector3(size, 1f, size);
        selectionPlane.GetComponent<Renderer>().material = selectionMat;
        Destroy(selectionPlane.GetComponent<Collider>());
        selectionPlane.transform.position = new Vector3(selection.transform.position.x, 0.05f, selection.transform.position.z);
    }

    void UpdateSelectionPlane()
    {
        if(selection == null)
        {
            if (selectionPlane != null)
                Destroy(selectionPlane);
            return;
        }
        if (selectionPlane == null)
            return;
        selectionPlane.transform.position = new Vector3(selection.transform.position.x, 0.05f, selection.transform.position.z);
    }

    void UpdateSelectionText()
    {
        if (selection == null || infoText == null)
        {
            infoText.text = "No selection";
            return;
        }

        string[] info = null;
        if(selection.GetComponent<Harvester>() != null)
        {
            info = selection.GetComponent<Harvester>().InfoText();
        } else if (selection.GetComponent<Turret>() != null)
        {
            info = selection.GetComponent<Turret>().InfoText();
        } else if (selection.GetComponent<Hive>() != null)
        {
            info = selection.GetComponent<Hive>().InfoText();
        } else if (selection.GetComponent<Refinery>() != null)
        {
            info = selection.GetComponent<Refinery>().InfoText();
        } else if (selection.GetComponent<Ant>() != null)
        {
            info = selection.GetComponent<Ant>().InfoText();
        } else if (selection.GetComponent<WarriorAnt>() != null)
        {
            info = selection.GetComponent<WarriorAnt>().InfoText();
        }
        if (info == null)
            return;
        string text = "";
        foreach (string line in info)
            text += line + "\n";
        infoText.text = text;
    }

    public void AddRessources(float a)
    {
        ressources += a;
    }

    public bool PayForPlate()
    {
        if(ressources >= plateCost)
        {
            ressources -= plateCost;
            return true;
        }
        return false;
    }

    public void BuildPhasmaTurret()
    {
        buildButtons.SetActive(false);
        if (selection.GetComponent<TurretSpot>() == null)
            return;
        if(ressources >= phasmaTurretCosts)
        {
            Instantiate(phasmaTurret, selection.transform.position, Quaternion.identity);
            ressources -= phasmaTurretCosts;
        }
        
    }

    public void BuildFlameTurret()
    {
        buildButtons.SetActive(false);
        if (selection.GetComponent<TurretSpot>() == null)
            return;
        if (ressources >= flameTurretCosts)
        {
            Instantiate(flameTurret, selection.transform.position, Quaternion.identity);
            ressources -= flameTurretCosts;
        }

    }

    public void BuildSniperTurret()
    {
        buildButtons.SetActive(false);
        if (selection.GetComponent<TurretSpot>() == null)
            return;
        if (ressources >= sniperTurretCosts)
        {
            Instantiate(sniperTurret, selection.transform.position, Quaternion.identity);
            ressources -= sniperTurretCosts;
        }
    }

    public void CancleBuild()
    {
        buildButtons.SetActive(false);
    }

    public void CancleRepair()
    {
        repairButtons.SetActive(false);
    }

    public void RepairTurret()
    {
        if(selection.GetComponent<Turret>() != null)
        {
            if(ressources > 0)
            {
                ressources -= 1;
                selection.GetComponent<Turret>().Repair(5.0f);
            }
        } else if(selection.GetComponent<Refinery>() != null)
        {
            if (ressources > 0)
            {
                ressources -= 1;
                selection.GetComponent<Refinery>().Repair(5.0f);
            }
        }
    }

    public bool Pay(float amount)
    {
        if(ressources >= amount)
        {
            ressources -= amount;
            return true;
        } else
        {
            return false;
        }
    }
}
