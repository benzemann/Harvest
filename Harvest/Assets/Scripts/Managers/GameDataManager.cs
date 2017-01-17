using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviour {

    [Header("Logging variables")]
    [SerializeField, Tooltip("Whether it should log game data to the database or not")]
    private bool shouldLog;
    [SerializeField, Tooltip("The time in seconds between each upload.")]
    private float uploadRate;
    private float lastUpload;
    [SerializeField, Tooltip("Whether or not to log when playing in editor mode.")]
    private bool shouldLogInEditor;
    // Unique guid for this play session
    private string guid;

    public string GameTimeString
    {
        get
        {
            int minutes = (int)Mathf.Floor(Time.time / 60f);
            int hours = (int)(minutes / 60f);
            minutes -= hours * 60;
            int seconds = (int)(Time.time - (minutes * 60));
            return seconds + ":" + minutes + ":" + hours;
        }
    }

    // Use this for initialization
    void Start () {
        guid = System.Guid.NewGuid().ToString();
    }
	
	// Update is called once per frame
	void Update () {
        // Should not be here, but at the moment its fine
        if ( Time.time - lastUpload >= uploadRate)
        {
            if ((Application.isEditor && shouldLogInEditor) || !Application.isEditor )
                StartCoroutine(UploadGameData());
            lastUpload = Time.time;
        }

        if (Input.GetKeyUp("escape"))
        {
            StartCoroutine(UploadGameData(true));
        }
    }


    /// <summary>
    /// Uploads game data to sql database.
    /// </summary>
    /// <returns></returns>
    IEnumerator UploadGameData(bool shouldExitOnComplete=false)
    {
        WWWForm form = new WWWForm();

        // Get a unique id for this device, used to identify the same player playing different sessions
        var id = SystemInfo.deviceUniqueIdentifier;

        form.AddField("GUID", guid);
        form.AddField("Id", id);
        form.AddField("DateTime", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        form.AddField("GameTime", GameTimeString);
        form.AddField("Duration", (int)Time.time);

        WWW w = new WWW("2.110.192.183/db/upload_game_data.php", form);

        yield return w;

        if (!string.IsNullOrEmpty(w.error))
        {
            Debug.Log(w.error);
        }
        else
        {
            Debug.Log("Upload of game data done.");
            if (shouldExitOnComplete)
            {
                Debug.Log("Will now exit...");
                Application.Quit();
            }
        }
    }
}
