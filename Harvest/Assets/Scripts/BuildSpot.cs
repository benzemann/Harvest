using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UIFollower)), RequireComponent(typeof(Selectable))]
public class BuildSpot : MonoBehaviour {
    [SerializeField]
    private GameObject turretPrefab;
    [SerializeField]
    private GameObject buttonGroup;
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private float buildTime;
    [Header("Animation variables")]
    [SerializeField]
    private float animationSpeed;
    [SerializeField]
    private float sizeStep;
    [SerializeField]
    private Vector3 buildBarPos;
    [SerializeField]
    private float buildBarScale;

    public Canvas Canvas { get { return canvas; } }
    public Vector3 BuildBarPos { get { return buildBarPos; } }
    public float BuildBarScale {  get { return buildBarScale; } }
    public float SizeStep {  get { return sizeStep; } }
    public bool IsOccupied { get { return isOccupied; } set { isOccupied = value; } }
    public float PercentageDone { get { return percentageDone; } }

    private GameObject _buttonGroup;
    private bool isButtonsVisible = false;
    private Selectable selectable;
    private bool isOccupied;
    private float percentageDone;
    private GameObject turretGo;

    // Use this for initialization
	void Start () {
        selectable = GetComponent<Selectable>();
	}
	
	// Update is called once per frame
	void Update () {
        if(!isButtonsVisible && selectable.IsSelected && !isOccupied)
        {
            ShowButtons();
            isButtonsVisible = true;
        } else if(isButtonsVisible && !selectable.IsSelected)
        {
            HideButtons();
            isButtonsVisible = false;
        }

        if(turretGo == null && isOccupied)
        {
            isOccupied = false;
        }
	}

    public void StartBuild()
    {
        var turret = Instantiate(turretPrefab, this.transform.position + (Vector3.down * turretPrefab.transform.localScale.y * 2), Quaternion.identity) as GameObject;
        turret.transform.SetParent(this.transform);
        turret.GetComponent<TargetFinder>().enabled = false;
        turret.GetComponent<TargetFinder>().ClearTarget();

        percentageDone = 0f;
        StartCoroutine(BuildTurret(turret));
        
        turretGo = turret;
    }

    IEnumerator BuildTurret(GameObject turret)
    {
        var timeSpend = 0f;
        while(timeSpend < buildTime && turret != null)
        {
            percentageDone = timeSpend / buildTime;
            turret.transform.localPosition = new Vector3(turret.transform.localPosition.x,
                (turretPrefab.transform.localScale.y * -20f) * (1f - percentageDone),
                turret.transform.localPosition.z);
            yield return new WaitForSeconds(0.1f);
            timeSpend += 0.1f;
        }
        if(turret != null)
        {
            turret.GetComponent<TargetFinder>().enabled = true;
            percentageDone = 1f;
        }
        Destroy(_buttonGroup);
    }

    void ShowButtons()
    {
        if(_buttonGroup == null)
        {
            _buttonGroup = Instantiate(buttonGroup, canvas.transform) as GameObject;
            GetComponent<UIFollower>().SetUIElement(_buttonGroup);
            for(int i = 0; i < _buttonGroup.transform.childCount; i++)
            {
                var child = _buttonGroup.transform.GetChild(i);
                if (child.gameObject.GetComponent<BuildButton>() != null)
                {
                    child.gameObject.GetComponent<BuildButton>().BuildSpot = this;
                }
            }
        }

        _buttonGroup.transform.localScale = Vector3.zero;
        StartCoroutine(ExpandButtonGroup());
    }

    public void HideButtons()
    {
        if(_buttonGroup == null)
        {
            return;
        }
        StartCoroutine(SwrinkButtonGroup());
    }

    IEnumerator SwrinkButtonGroup()
    {
        while(_buttonGroup.transform.localScale.x > 0f)
        {
            _buttonGroup.transform.localScale -= new Vector3(sizeStep, sizeStep, sizeStep);
            yield return new WaitForSeconds(animationSpeed);
        }
        _buttonGroup.transform.localScale = Vector3.zero;
    }

    IEnumerator ExpandButtonGroup()
    {
        while(_buttonGroup.transform.localScale.x < 1f)
        {
            _buttonGroup.transform.localScale += new Vector3(sizeStep, sizeStep, sizeStep);
            yield return new WaitForSeconds(animationSpeed);
        }
        _buttonGroup.transform.localScale = Vector3.one;
    }

}
