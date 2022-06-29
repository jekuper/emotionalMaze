using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class settingsManager : MonoBehaviour
{
    public Slider starSlider, campSlider, enemiesSlider;
    public Toggle starSearch, keySearch, joysticksSwap, muteAudio;

    private settingsProfile dublicate;
    void Start()
    {
        dublicate = Globals.mainSettings;
        starSlider.value = dublicate.starCount;
        campSlider.value = dublicate.campCount;
        enemiesSlider.value = dublicate.enemiesCount;
        starSearch.isOn = dublicate.starSearchHelp;
        keySearch.isOn = dublicate.keySearchHelp;
        joysticksSwap.isOn = dublicate.swapJoysticks;
        muteAudio.isOn = dublicate.muteAudio;
    }
    public void SaveSettings() {
        dublicate.starCount = (int)starSlider.value;
        dublicate.campCount = (int)campSlider.value;
        dublicate.enemiesCount = (int)enemiesSlider.value;
        dublicate.starSearchHelp = starSearch.isOn;
        dublicate.keySearchHelp = keySearch.isOn;
        dublicate.swapJoysticks = joysticksSwap.isOn;
        dublicate.muteAudio = muteAudio.isOn;

        string encrypted = JsonConvert.SerializeObject(Globals.mainSettings);
        fileManager.SaveId("settings", encrypted);
        Globals.mainSettings = dublicate;
        Globals.audioManager.updateMuteState ();
    }
    public void ClearData()
    {
        Globals.isFirstLoad = false;
        fileManager.DeleteFile("settings");
        fileManager.DeleteFile("firstLoad");
        fileManager.DeleteFile("scores");
    }
}
