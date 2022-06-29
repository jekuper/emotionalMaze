using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class loader : MonoBehaviour
{
//    private void Awake() {
//        DontDestroyOnLoad(gameObject);
//    }
    void Start()
    {
        Load();
    }
    public void Load()
    {
        if (!Globals.isFirstLoad)
        {
            if (fileManager.fileExist("firstLoad"))
            {
                Globals.isFirstLoad = false;
            }
            else
            {
                Globals.isFirstLoad = true;
                fileManager.SaveId("firstLoad", ".");
            }
        }
        string[] data = fileManager.LoadId("settings");
        string jsonData = fileManager.ConnectLines(data, "\n");
        settingsProfile s = JsonConvert.DeserializeObject<settingsProfile>(jsonData);
        Globals.mainSettings = s;
        Globals.audioManager.updateMuteState ();
    }
}
