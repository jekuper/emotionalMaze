using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject dictionaryPanel;
    public GameObject outsidePauseButton;
    public static bool isPaused = false;

    public void Pause()
    {
        if (isPaused)
            return;
        outsidePauseButton.SetActive(false);
        isPaused = true;
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
    }
    public void Resume()
    {
        if (!isPaused)
            return;
        outsidePauseButton.SetActive(true);
        isPaused = false;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
    }
    public void OpenDictionary()
    {
        dictionaryPanel.SetActive(true);
    }
    public void CloseDictionary()
    {
        dictionaryPanel.SetActive(false);
    }
    public void Quit()
    {
        isPaused = false;
        Time.timeScale = 1f;
        Globals.GoToScene("menu");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused == false)
                Pause();
            else
                Quit();
        }
    }
}
