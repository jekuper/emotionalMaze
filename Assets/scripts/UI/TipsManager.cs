using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TipsManager : MonoBehaviour
{
    public Action _callback;
    public TextMeshProUGUI tipText;

    [SerializeField] private TextMeshProUGUI nextButtonText;
    [SerializeField] private CanvasGroup canvasGroup;

    private List<string> _tipsTexts = new List<string>();
    private int textIndex = 0;

    private void Awake () {
    }

    public void ShowTip (List<string> tipTexts, Action callback = null) {
        Time.timeScale = 0;
        StartCoroutine (ShowAnimation ());
        _callback = callback;
        _tipsTexts = tipTexts;
        textIndex = 0;
        NextText ();
    }

    public void NextText () {
        if (_tipsTexts.Count == 0) {
            Debug.LogError ("nothing to display");
            return;
        }
        if (textIndex == _tipsTexts.Count) {
            Time.timeScale = 1;
            StartCoroutine(HideAnimation ());
        } else {
            if (textIndex == _tipsTexts.Count - 1) {
                nextButtonText.text = "-OK-";
            } else {
                nextButtonText.text = "-Next-";
            }
            tipText.text = _tipsTexts[textIndex];
            textIndex++;
        }
    }
    private IEnumerator HideAnimation () {
        canvasGroup.blocksRaycasts = false;
        float timing = 0.3f;

        float speed = -canvasGroup.alpha / timing;
        while (canvasGroup.alpha > 0.1f) {
            float newAlpha = canvasGroup.alpha + speed * 0.02f;
            canvasGroup.alpha = newAlpha;
            yield return new WaitForSecondsRealtime (0.02f);
        }
        canvasGroup.alpha = 0;

        if (_callback != null)
            _callback ();
    }
    private IEnumerator ShowAnimation () {
        canvasGroup.blocksRaycasts = true;
        float timing = 0.3f;

        float speed = 1 / timing;
        while (canvasGroup.alpha < 1f) {
            float newAlpha = canvasGroup.alpha + speed * 0.02f;
            canvasGroup.alpha = newAlpha;
            yield return new WaitForSecondsRealtime (0.02f);
        }
        canvasGroup.alpha = 1;
    }
}
