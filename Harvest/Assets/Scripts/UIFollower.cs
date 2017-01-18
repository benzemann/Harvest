using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollower : MonoBehaviour {

    #region Mandatory variables
    [Header("Mandatory variables")]
    [SerializeField, Tooltip("The renderer the screen space bounding box should be based on")]
    private Renderer ren;
    [SerializeField, Tooltip("The canvas the bar and boundingBox should be instantiated on")]
    private Canvas canvas;
    #endregion

    #region Bar variables
    [Header("Bar variables")]
    [SerializeField, Tooltip("The prefab of the bar")]
    private GameObject barPrefab;
    [SerializeField, Tooltip("Decide whether to scale the bar based on the screen size of the object or keep the original size")]
    private bool scaleBar;
    #endregion

    #region Bounding box variables
    [Header("Bounding box variables")]
    [SerializeField, Tooltip("The prefab of the bounding box")]
    private GameObject boundingBoxPrefab;
    [SerializeField, Tooltip("If the boundingbox should be shown always or only when selected")]
    private bool showOnlyOnSelected;
    [SerializeField, Tooltip("Whether to scale the bounding box based on the screen size of the object or keep the original size")]
    private bool scaleBoundingBox;
    #endregion

    #region private
    private GameObject bar;
    private GameObject boundingBox;
    private Rect screenRect;
    private Vector3[] pts = new Vector3[8];
    #endregion

    // Use this for initialization
    void Start () {
        if(barPrefab != null)
            bar = Instantiate(barPrefab, canvas.transform) as GameObject;
        if (boundingBoxPrefab != null)
            boundingBox = Instantiate(boundingBoxPrefab, canvas.transform) as GameObject;
    }

    private void Update()
    {
        if(screenRect != null && scaleBar)
            ResizeBarToRect(screenRect, bar.GetComponent<RectTransform>());
    }

    // Update is called once per frame
    void LateUpdate () {
        FollowObjectOnScreen();
	}

    /// <summary>
    /// Updates the position of the bar on the canvas. 
    /// </summary>
    void FollowObjectOnScreen()
    {
        Rect? r = GetScreenBoundingBox(0.2f);
        if (r != null)
        {
            screenRect = r.Value;
            if (bar != null)
            {
                bar.transform.position = Camera.main.WorldToScreenPoint(this.transform.position);
                bar.transform.localPosition = new Vector3(bar.transform.localPosition.x, bar.transform.localPosition.y + (screenRect.height), bar.transform.localPosition.z);
            }
            if(boundingBox != null)
            {
                // Disable/Enable renderer based
                if (showOnlyOnSelected)
                    if (GetComponent<Selectable>() != null && GetComponent<Selectable>().IsSelected)
                        boundingBox.gameObject.SetActive(true);
                    else
                        boundingBox.gameObject.SetActive(false);
                // Set boundingbox pos and size
                boundingBox.transform.position = Camera.main.WorldToScreenPoint(this.transform.position);
                if(scaleBoundingBox)
                    boundingBox.GetComponent<RectTransform>().sizeDelta = screenRect.size;
            }            
        } else
        {
            Debug.LogError("Could not calculate screen bounding box. Set variables correct!");
        }
    }

    /// <summary>
    /// Get the screen space rect of the object.
    /// </summary>
    /// <param name="margin">margin around the object and the rect border</param>
    /// <returns>The screen space rect</returns>
    private Rect? GetScreenBoundingBox(float margin)
    {
        if (ren == null) return null;

        Bounds b = ren.bounds;
        Camera cam = Camera.main;

        //The object is behind us
        if (cam.WorldToScreenPoint(b.center).z < 0) return null;

        //All 8 vertices of the bounds
        pts[0] = cam.WorldToScreenPoint(new Vector3(b.center.x + b.extents.x, b.center.y + b.extents.y, b.center.z + b.extents.z));
        pts[1] = cam.WorldToScreenPoint(new Vector3(b.center.x + b.extents.x, b.center.y + b.extents.y, b.center.z - b.extents.z));
        pts[2] = cam.WorldToScreenPoint(new Vector3(b.center.x + b.extents.x, b.center.y - b.extents.y, b.center.z + b.extents.z));
        pts[3] = cam.WorldToScreenPoint(new Vector3(b.center.x + b.extents.x, b.center.y - b.extents.y, b.center.z - b.extents.z));
        pts[4] = cam.WorldToScreenPoint(new Vector3(b.center.x - b.extents.x, b.center.y + b.extents.y, b.center.z + b.extents.z));
        pts[5] = cam.WorldToScreenPoint(new Vector3(b.center.x - b.extents.x, b.center.y + b.extents.y, b.center.z - b.extents.z));
        pts[6] = cam.WorldToScreenPoint(new Vector3(b.center.x - b.extents.x, b.center.y - b.extents.y, b.center.z + b.extents.z));
        pts[7] = cam.WorldToScreenPoint(new Vector3(b.center.x - b.extents.x, b.center.y - b.extents.y, b.center.z - b.extents.z));

        //Get them in screen space
        for (int i = 0; i < pts.Length; i++) pts[i].y = Screen.height - pts[i].y;

        //Calculate the min and max positions
        Vector3 min = pts[0];
        Vector3 max = pts[0];
        for (int i = 1; i < pts.Length; i++)
        {
            min = Vector3.Min(min, pts[i]);
            max = Vector3.Max(max, pts[i]);
        }
        
        //Construct a rect of the min and max positions and apply some margin
        Rect r = Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        r.xMin -= margin;
        r.xMax += margin;
        r.yMin -= margin;
        r.yMax += margin;

        return r;
    }

    /// <summary>
    /// Resizes the bar based on the given rect
    /// </summary>
    /// <param name="rec">The rect which the bar should resize</param>
    /// <param name="rt">The recttransform which should be resized</param>
    /// <param name="childIndx">childIndex should not be set only in this function (recursiv)</param>
    void ResizeBarToRect(Rect rec, RectTransform rt, int childIndx=-1)
    {
        if (rt == null) return;
        // Resize rt
        var sizeDelta = rt.GetComponent<RectTransform>().sizeDelta;
        rt.GetComponent<RectTransform>().sizeDelta = new Vector2(rec.width, sizeDelta.y);
        if (childIndx > 0)
        {
            rt.transform.localPosition = new Vector3(rec.width * 0.5f, 0f, 0f);
        }

        // Loop all children
        for (int i = 0; i < rt.transform.childCount; i++)
        {
            var child = rt.transform.GetChild(i);
            if (child.GetComponent<RectTransform>() == null) break;
            
            ResizeBarToRect(rec, child.GetComponent<RectTransform>(), childIndx + 1);
        }

    }
}
