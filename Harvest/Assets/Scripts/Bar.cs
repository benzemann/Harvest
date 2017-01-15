using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class Bar : MonoBehaviour {

    [SerializeField, Range(0.0f, 1.0f), Tooltip("Percentage of how much the bar is filled")]
    private float _value;
    [SerializeField, Tooltip("The minimum value of the bar")]
    private float minValue;
    [SerializeField, Tooltip("Whether the bar should be hidden when the value is under minimum value")]
    private bool hideOnMin;
    [SerializeField, Tooltip("The bar which will be scaled acordingly to value")]
    private RectTransform slider;
    
    private float maxWidth;
    
    public float Value { set { _value = value; } }

	// Use this for initialization
	void Start () {
        // Chache max width
        maxWidth = GetComponent<RectTransform>().rect.width;

        // If slider has not been assigned, try to use first child
        if(slider == null)
        {
            var child = transform.GetChild(0);
            if (child.GetComponent<RectTransform>() != null)
            {
                slider = child.GetComponent<RectTransform>();
            }
        }
        

    }
	
	// Update is called once per frame
	void Update () {
		// Set the new with of the slider
        if(slider != null)
        {
            if (_value <= minValue)
            {
                if(hideOnMin)
                    slider.gameObject.SetActive(false);
                _value = minValue;
            } else
            {
                slider.gameObject.SetActive(true);
                slider.sizeDelta = new Vector2((_value * maxWidth), slider.sizeDelta.y);
            }
        }
	}

}
