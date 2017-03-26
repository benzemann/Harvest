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
    [SerializeField, Tooltip("The type of the bar")]
    private BarTypes barType;
    [SerializeField, Tooltip("Scale the UI element based or keep size fixed")]
    private bool scaleBar;
    [SerializeField, Range(0f, 5f), Tooltip("The max size of the bar.")]
    private float maxScale;
    [SerializeField, Range(0f, 1f), Tooltip("The initial size of the bar.")]
    private float minScale;
    [SerializeField, Range(0f, 1f), Tooltip("How much the bar scaled pr. unit away from max distance")]
    float scalePrUnit;
    [SerializeField, Range(0f, 100f), Tooltip("The distance where the UI element will be at minscale.")]
    private float maxDistance;
    [SerializeField, Tooltip("Offset from center of the object.")]
    private Vector2 offsetFromCenter;
    #endregion

    #region private
    private GameObject bar;
    #endregion

    public enum BarTypes {
        None,
        Health,
        Ressources
    }

    // Use this for initialization
    void Start () {
        if(barPrefab != null)
            bar = Instantiate(barPrefab, canvas.transform) as GameObject;
            
    }

    private void Update()
    {
        switch (barType)
        {
            case BarTypes.Health:
                if (GetComponent<Health>() != null)
                    bar.GetComponent<Bar>().Value = GetComponent<Health>().Percentage;
                else
                    Debug.LogWarning("Bar type is set to health but there is no healt component attached to gameobject.");
                break;
            case BarTypes.Ressources:
                if(GetComponent<ResourceStorage>() != null)
                {
                    bar.GetComponent<Bar>().Value = GetComponent<ResourceStorage>().Percentage;
                } else
                {
                    Debug.LogWarning("Bar type is set to resource but there is no resource storage component attached to gameobject.");
                }
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void LateUpdate () {
        ResizeAndMoveBar();
    }
    
    /// <summary>
    /// Resize bar based on distance to camera
    /// </summary>
    void ResizeAndMoveBar()
    {

        if (bar != null)
        {
            float disToCam = Vector3.Distance(transform.position, Camera.main.transform.position);
            if (scaleBar)
            {
                float scale = Mathf.Max(maxDistance - disToCam, 0f) * scalePrUnit + minScale;
                if (scale > maxScale) scale = maxScale;
                bar.transform.localScale = Vector3.one * scale;
            }

            float barHeightOffset = (offsetFromCenter.y);
            bar.transform.position = Camera.main.WorldToScreenPoint(ren.transform.position);
            bar.transform.localPosition = new Vector3(bar.transform.localPosition.x + offsetFromCenter.x, bar.transform.localPosition.y + barHeightOffset, bar.transform.localPosition.z);
        }
    }

    private void OnDestroy()
    {
        Destroy(bar);
    }
}
