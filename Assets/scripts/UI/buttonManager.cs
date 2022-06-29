using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonManager : MonoBehaviour
{
    public void GoToScene(string name) {
        Globals.GoToScene(name);
    }
    public void StopMusic(string name)
    {
        Globals.audioManager.Stop(name);
    }
    public void StartMusic(string name)
    {
        Globals.audioManager.Play(name);
    }
}
