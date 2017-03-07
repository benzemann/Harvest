using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPress : MonoBehaviour
{
    [Header("Fill expand")]
    [SerializeField]
    private GameObject fill;
    [SerializeField]
    private Vector2 filledSize;
    [SerializeField]
    private float fillSpeed;
    [Header("Background expand")]
    [SerializeField]
    private float delay;
    [SerializeField]
    private GameObject background;
    [SerializeField]
    private Vector2 bckGroundSize;
    [SerializeField]
    private float bckSpeed;
    [Header("Animation")]
    [SerializeField]
    private float animationUpdateRate;
    // Use this for initialization
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(TaskOnClick);
    }

    public void TaskOnClick()
    {
        StartCoroutine(ExpandFill());
    }

    IEnumerator ExpandFill()
    {
        while (true)
        {
            fill.GetComponent<RectTransform>().sizeDelta += new Vector2(fillSpeed, fillSpeed) * Time.deltaTime;
            if (fill.GetComponent<RectTransform>().sizeDelta.x > filledSize.x)
            {
                fill.GetComponent<RectTransform>().sizeDelta = filledSize;
                yield return new WaitForSeconds(delay);

                StartCoroutine(ExpandBackground());
                yield return null;
            }
            yield return new WaitForSeconds(animationUpdateRate);
        }
    }

    IEnumerator ExpandBackground()
    {
        while (true)
        {
            var sd = new Vector2(bckSpeed, 0f) * Time.deltaTime;
            background.GetComponent<RectTransform>().sizeDelta += sd;
            //background.GetComponent<RectTransform>().
            if (background.GetComponent<RectTransform>().sizeDelta.x > bckGroundSize.x)
            {
                background.GetComponent<RectTransform>().sizeDelta = bckGroundSize;
                yield return null;
            }
            yield return new WaitForSeconds(animationUpdateRate);

        }
    }
}
