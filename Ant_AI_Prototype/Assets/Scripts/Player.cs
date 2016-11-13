using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Player : MonoBehaviour {

    public float plateCost;
    public float startRessources;
    GameObject selection;
    public Material selectionMat;
    Camera camera;
    GameObject selectionPlane;
    public Text infoText;
    public Text ressourcesText;
    public List<GameObject> turrets;
    public GameObject drill;
    public GameObject[] buildings;
    public GameObject buttonPrefab;
    public GameObject layoutGroup;
    float ressources;
    public Text objectiveText;
    public GameObject[] objectives;
    GameObject[] objectivesGO;
    public Text victoryText;
    // Use this for initialization
    void Start () {
        ressources = startRessources;
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        objectivesGO = new GameObject[objectives.Length];
        int indx = 0;
        foreach(GameObject objective in objectives)
        {
            objectivesGO[indx] = Instantiate(objective) as GameObject;
            indx++;
        }
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
        foreach (GameObject objective in objectivesGO)
            objective.GetComponent<Objectives>().UpdateObjective(0);
        UpdateObjectiveText();
        ressourcesText.text = ressources.ToString();
    }

    void ShowTurretBuildButtons()
    {
        if(selection.GetComponent<TurretSpot>().HasPlate() && !selection.GetComponent<TurretSpot>().hasTurret && !selection.GetComponent<TurretSpot>().isDrillSpot)
        {
            int i = 0;
            float widthOffset = 0f;
            foreach (GameObject t in turrets)
            {
                GameObject newButton = Instantiate(buttonPrefab, transform.position, Quaternion.identity) as GameObject;
                newButton.transform.parent = layoutGroup.transform;
                newButton.transform.GetChild(0).GetComponent<Text>().text = t.GetComponent<Turret>().textOnButton.Replace("NEWLINE", "\n");
                newButton.transform.localPosition = new Vector3(widthOffset, 0f, 0f);
                widthOffset += newButton.GetComponent<RectTransform>().rect.width / 1.5f;
                int x = i;
                newButton.GetComponent<Button>().onClick.AddListener(delegate { int y = x; BuildTurret(y); });
                i++;
            }
        }  else if (selection.GetComponent<TurretSpot>().isDrillSpot && !selection.GetComponent<TurretSpot>().hasTurret)
        {
            GameObject newButton = Instantiate(buttonPrefab, transform.position, Quaternion.identity) as GameObject;
            newButton.transform.parent = layoutGroup.transform;
            newButton.transform.GetChild(0).GetComponent<Text>().text = drill.GetComponent<Turret>().textOnButton;
            newButton.transform.localPosition = new Vector3(0f, 0f, 0f);
            newButton.GetComponent<Button>().onClick.AddListener(delegate { BuildDrill(); });
        }
    }

    public void BuildDrill()
    {
        CancleBuild();
        if (selection.GetComponent<TurretSpot>() == null)
            return;
        
        if (ressources >= drill.GetComponent<Turret>().costs)
        {
            selection.GetComponent<TurretSpot>().hasTurret = true;
            GameObject newTurret = Instantiate(drill, selection.transform.position, Quaternion.identity) as GameObject;
            selection.GetComponent<TurretSpot>().SetTurret(newTurret);
            GameObject[] upgrades = GameObject.FindGameObjectsWithTag("Upgrade");
            foreach (GameObject upgrade in upgrades)
            {
                upgrade.GetComponent<Upgrade>().ApplyUpradeToThis(newTurret);
            }
            ressources -= drill.GetComponent<Turret>().costs;
        }
    }

    void ShowBuildingButtons()
    {
        if (!selection.GetComponent<BuildingSpot>().hasBuilding)
        {
            int i = 0;
            float widthOffset = 0f;
            foreach(GameObject b in buildings)
            {
                GameObject newButton = Instantiate(buttonPrefab, transform.position, Quaternion.identity) as GameObject;
                newButton.transform.parent = layoutGroup.transform;
                newButton.transform.GetChild(0).GetComponent<Text>().text = b.GetComponent<Building>().textOnButton.Replace("NEWLINE", "\n");
                newButton.transform.localPosition = new Vector3(widthOffset, 0f, 0f);
                widthOffset += newButton.GetComponent<RectTransform>().rect.width / 1.5f;
                int x = i;
                newButton.GetComponent<Button>().onClick.AddListener(delegate { int y = x; BuildBuilding(y); } );
                i++;
            }
        }
    }

    public void CheckVictory()
    {
        bool isAllComplete = true;
        foreach (GameObject objective in objectivesGO)
            if (!objective.GetComponent<Objectives>().isComplete)
                isAllComplete = false;
        if(isAllComplete)
            victoryText.gameObject.SetActive(true);
    }

    public float GetRessources()
    {
        return ressources;
    }

    void ShowUpgradeButtons()
    {
        if (!selection.GetComponent<Building>().isReady)
            return;
        GameObject[] upgrades = selection.GetComponent<Building>().upgrades;
        int i = 0;
        float widthOffset = 0f;
        foreach (GameObject upgrade in upgrades)
        {
            if (upgrade.GetComponent<Upgrade>().hasBeenApplied)
            {
                i++;
                continue;
            }
            GameObject newButton = Instantiate(buttonPrefab, transform.position, Quaternion.identity) as GameObject;
            newButton.transform.parent = layoutGroup.transform;
            newButton.transform.GetChild(0).GetComponent<Text>().text = upgrade.GetComponent<Upgrade>().textOnButton.Replace("NEWLINE", "\n");
            newButton.transform.localPosition = new Vector3(widthOffset, 0f, 0f);
            widthOffset += newButton.GetComponent<RectTransform>().rect.width / 1.5f;
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

    void UpdateObjectiveText()
    {
        StringBuilder sb = new StringBuilder("OBJECTIVES: \n");
        foreach (GameObject objective in objectivesGO)
            sb.Append(objective.GetComponent<Objectives>().GetText() + "\n");
        objectiveText.text = sb.ToString();
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
        foreach (GameObject objective in objectivesGO)
            objective.GetComponent<Objectives>().UpdateObjective(a);
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
