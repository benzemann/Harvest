using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour {

    [SerializeField, Tooltip("The speed of the camera mvements")]
    private float speed;
    [SerializeField, Tooltip("The zoom speed of the camera")]
    private float zoomSpeed;
    [SerializeField, Tooltip("The camera gameobject")]
    private GameObject camera;
    /*[SerializeField, Tooltip("The position the camera will start in")]
    private int startingIndx;
    [SerializeField, Tooltip("A list of camera positions. Transform is a child transform of the camera. Layer is whether the position is after or before the start pos (start position of the camera is layer 0, e.g. you need to zoom out to go to layer -1 and zoom in to go to layer 1 from layer 0)")]
    private Transform[] cameraZoomPositions;*/

    private int currentIndx;
    private int posCount;

    [System.Serializable]
    private class CameraPosition
    {
        public Transform transform;
        public bool isStartingPos;
    }

	// Use this for initialization
	void Start () {
        if (camera == null)
            Debug.LogError("There is no camera attached to the camera controller!");

        /*currentIndx = startingIndx;

        posCount = cameraZoomPositions.Length;

        camera.transform.position = cameraZoomPositions[currentIndx].transform.position;
        camera.transform.rotation = cameraZoomPositions[currentIndx].transform.rotation;*/
	}
	
	// Update is called once per frame
	void Update () {
        var zoom = Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomSpeed;

        // Get translation of camera in the xz plane
        Vector3 translation = new Vector3(
            Input.GetAxis("Horizontal") * Time.deltaTime * speed,
            -zoom,
            Input.GetAxis("Vertical") * Time.deltaTime * speed
            );
        // Move camera
        this.transform.position += translation;

        /*

        if(zoom < 0f)
        {
            // Zoom out
            if(currentIndx < posCount - 1)
            {
                StartCoroutine(MoveCameraToPosition(camera.transform.position, camera.transform.rotation, cameraZoomPositions[currentIndx + 1].transform));
                currentIndx += 1;
            }
        } else if (zoom > 0f)
        {
            // Zoom in
            if(currentIndx > 0)
            {
                StartCoroutine(MoveCameraToPosition(camera.transform.position, camera.transform.rotation, cameraZoomPositions[currentIndx-1].transform));
                currentIndx -= 1;
            }

        }*/
	}
    /*
    IEnumerator MoveCameraToPosition(Vector3 startPos, Quaternion startRotation, Transform target)
    {
        float step = (speed / (startPos - target.position).magnitude) * Time.fixedDeltaTime;
        float t = 0;
        while (t <= 1.0f)
        {
            t += step; // Goes from 0 to 1, incrementing by step each time
            camera.transform.position = Vector3.Lerp(startPos, target.position, t); // Move objectToMove closer to b
            camera.transform.rotation = Quaternion.Lerp(startRotation, target.rotation, t);
            yield return new WaitForFixedUpdate();         // Leave the routine and return here in the next frame
        }
        camera.transform.position = target.position;
        camera.transform.rotation = target.rotation;
    }*/
}
