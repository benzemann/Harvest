using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BuildButton : MonoBehaviour {
    
    public BuildSpot BuildSpot { get; set; }

	// Use this for initialization
	void Start () {

        GetComponent<Button>().onClick.AddListener(OnClick);

    }

    IEnumerator Swrink()
    {
        while(this.transform.localScale.x > BuildSpot.BuildBarScale)
        {
            this.transform.localScale -= new Vector3(BuildSpot.SizeStep, BuildSpot.SizeStep, BuildSpot.SizeStep);
            yield return new WaitForSeconds(0.01f);
        }
        this.transform.localScale = new Vector3(BuildSpot.BuildBarScale, BuildSpot.BuildBarScale, BuildSpot.BuildBarScale);
    }

    IEnumerator MoveToBarPos()
    {
        while(this.transform.localPosition != BuildSpot.BuildBarPos)
        {
            var desiredPos = (BuildSpot.BuildBarPos - this.transform.localPosition);
            if(desiredPos.magnitude > 5f)
            {
                desiredPos = desiredPos.normalized * 5f;
            }
            this.transform.localPosition += desiredPos;

            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator Building()
    {
        while(BuildSpot.PercentageDone < 1f)
        {
            yield return new WaitForSeconds(0.1f);
        }
        Destroy(this.gameObject);
    }

    void OnClick()
    {
        if (BuildSpot == null)
            return;
        var tmpGO = new GameObject();
        tmpGO.transform.SetParent(BuildSpot.Canvas.transform);
        tmpGO.transform.position = Vector3.zero;
        var oldPos = this.transform.localPosition;
        this.transform.SetParent(tmpGO.transform);
        this.transform.position = oldPos;
        var uiFollow = BuildSpot.gameObject.AddComponent<UIFollower>();
        uiFollow.SetUIElement(tmpGO);
        uiFollow.Renderer = BuildSpot.GetComponent<Renderer>();
        uiFollow.Canvas = BuildSpot.Canvas;
        uiFollow.OffsetFromCenter = new Vector2(0f, 2f);
        BuildSpot.HideButtons();
        StartCoroutine(Swrink());
        StartCoroutine(MoveToBarPos());
        BuildSpot.IsOccupied = true;
        BuildSpot.StartBuild();
        StartCoroutine(Building());
    }
}
