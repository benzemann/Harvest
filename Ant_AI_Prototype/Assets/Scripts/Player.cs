using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour {

    public float plateCost;
    public float startRessources;
    GameObject selection;
    public Material selectionMat;
    Camera camera;
    GameObject selectionPlane;
    public Text infoText;
    public Text ressourcesText;
    public GameObject[] turrets;
    public GameObject[] buildings;
    public GameObject buttonPrefab;
    public GameObject layoutGroup;
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
                        parentHit.tag == "Ants" ||
                        parentHit.tag == "BuildingSpot" ||
                        parentHit.tag == "Building")
                    {
                        //CancleRepair();
                        CancleBuild();
                        selection = parentHit.transform.gameObject;
                        if (selection.tag == "TurretSpot")
                            ShowTurretBuildButtons();
                        if (selection.tag == "BuildingSpot")
                            ShowBuildingButtons();
                        if (selection.tag == "Building")
                            ShowUpgradeButtons();
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
                        parentHit.tag == "Ants" ||
                        parentHit.tag == "BuildingSpot" ||
                        parentHit.tag == "Building")
                    {
                        //CancleRepair();
                        CancleBuild();
                        selection = parentHit;
                        if (selection.tag == "TurretSpot")
                            ShowTurretBuildButtons();
                        if (selection.tag == "BuildingSpot")
                            ShowBuildingButtons();
                        if (selection.tag == "Building")
                            ShowUpgradeButtons();
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
        ressourcesText.text = ressources.ToString();
    }

    void ShowTurretBuildButtons()
    {
        if(selection.GetComponent<TurretSpot>().HasPlate() && !selection.GetComponent<TurretSpot>().hasTurret)
        {
            int i = 0;
            foreach(GameObject t in turrets)
            {
                GameObject newButton = Instantiate(buttonPrefab, transform.position, Quaternion.identity) as GameObject;
                newButton.transform.parent = layoutGroup.transform;
                newButton.transform.GetChild(0).GetComponent<Text>().text = t.GetComponent<Turret>().textOnButton;
                int x = i;
                newButton.GetComponent<Button>().onClick.AddListener(delegate { int y = x; BuildTurret(y); });
                i++;
            }
        }  
    }

    void ShowBuildingButtons()
    {
        if (!selection.GetComponent<BuildingSpot>().hasBuilding)
        {
            int i = 0;
            foreach(GameObject b in buildings)
            {
                GameObject newButton = Instantiate(buttonPrefab, transform.position, Quaternion.identity) as GameObject;
                newButton.transform.parent = layoutGroup.transform;
                newButton.transform.GetChild(0).GetComponent<Text>().text = b.GetComponent<Building>().textOnButton;
                int x = i;
                newButton.GetComponent<Button>().onClick.AddListener(delegate { int y = x; BuildBuilding(y); } );
                i++;
            }
        }
    }

    void ShowUpgradeButtons()
    {
        if (!selection.GetComponent<Building>().isReady)
            return;
        GameObject[] upgrades = selection.GetComponent<Building>().upgrades;
        int i = 0;
        foreach(GameObject upgrade in upgrades)
        {
            if (upgrade.GetComponent<Upgrade>().hasBeenApplied)
            {
                i++;
                continue;
            }
            GameObject newButton = Instantiate(buttonPrefab, transform.position, Quaternion.identity) as GameObject;
            newButton.transform.parent = layoutGroup.transform;
            newButton.transform.GetChild(0).GetComponent<Text>().text = upgrade.GetComponent<Upgrade>().textOnButton;
            int x = i;
            newButton.GetComponent<Button>().onClick.AddListener(delegate { int y = x; ApplyUpgrade(y); });
            i++;
        }
    }

    void ApplyUpgrade(int index)
    {
        if(selection == null)
        {
            return;
        }
        
        if(ressources >= selection.GetComponent<Building>().upgrades[index].GetComponent<Upgrade>().costs)
        {
            ressources -= selection.GetComponent<Building>().upgrades[index].GetComponent<Upgrade>().costs;
            selection.GetComponent<Building>().upgrades[index].GetComponent<Upgrade>().hasBeenApplied = true;
            GameObject[] upgradableObjects = GameObject.FindGameObjectsWithTag("Player");
            foreach(GameObject g in upgradableObjects)
            {
                selection.GetComponent<Building>().upgrades[index].GetComponent<Upgrade>().ApplyUpradeToThis(g);
            }
            
        }
        CancleBuild();
    }
    

    void HideUpgradeButtons()
    {

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
        float size = 1f;
        if (selection.GetComponent<Renderer>() != null)
            size = selection.GetComponent<Renderer>().bounds.extents.magnitude * .25f;
        else
            size = selection.transform.GetChild(0).gameObject.GetComponent<Renderer>().bounds.extents.magnitude * .25f;
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

    public void BuildTurret(int indx)
    {
        CancleBuild();
        if (selection.GetComponent<TurretSpot>() == null)
            return;
        GameObject turret = turrets[indx];
        if(ressources >= turret.GetComponent<Turret>().costs)
        {
            selection.GetComponent<TurretSpot>().hasTurret = true;
            GameObject newTurret = Instantiate(turret, selection.transform.position, Quaternion.identity) as GameObject;
            selection.GetComponent<TurretSpot>().SetTurret(newTurret);
            GameObject[] upgrades = GameObject.FindGameObjectsWithTag("Upgrade");
            foreach (GameObject upgrade in upgrades)
            {
                upgrade.GetComponent<Upgrade>().ApplyUpradeToThis(newTurret);
            }
            ressources -= turret.GetComponent<Turret>().costs;
        }
    }

    public void BuildBuilding(int buildingIndex)
    {
        if(buildings.Length > buildingIndex)
        {
            if(ressources >= buildings[buildingIndex].GetComponent<Building>().costs)
            {
                selection.GetComponent<BuildingSpot>().hasBuilding = true;
                ressources -= buildings[buildingIndex].GetComponent<Building>().costs;
                GameObject b = Instantiate(buildings[buildingIndex], selection.transform.position, Quaternion.identity) as GameObject;

            }
        }
        CancleBuild();

    }

    public void CancleBuild()
    {
        for(int i = 0; i < layoutGroup.transform.childCount; i++)
        {
            Destroy(layoutGroup.transform.GetChild(i).gameObject);
        }
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
