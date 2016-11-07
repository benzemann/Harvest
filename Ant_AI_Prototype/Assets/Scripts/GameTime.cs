using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameTime : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
        int minutes = (int)Mathf.Floor( Time.time / 60f );
        int hours = (int)(minutes / 60f);
        minutes -= hours * 60;
        int seconds = (int)(Time.time - (minutes * 60));
        GetComponent<Text>().text = hours + ":" + minutes + ":" + seconds;
	}
}
