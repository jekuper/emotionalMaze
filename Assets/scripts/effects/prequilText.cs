using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class prequilText : MonoBehaviour
{
    public string[] data;
    public string sceneName = "main";
    public float targetFontSize = 50f;
    TextMeshProUGUI text;
    private void Start()
    {
        if (!Globals.isFirstLoad)
        {
            StartCoroutine(SwitchMusic());
        }
        else
        {
            Globals.isFirstLoad = false;
            text = GetComponent<TextMeshProUGUI>();
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
            StartCoroutine(Effect());
        }
    }
    private IEnumerator Effect()
    {
        yield return new WaitForSeconds(2.3f);
        for (int i = 0; i < data.Length; i++)
        {
            if (i == data.Length - 1)
                text.fontSize = targetFontSize;
            text.text = data[i];
            yield return StartCoroutine(cEffector.UiOpacity2(text, 2, 1));
            yield return new WaitForSeconds(2);
            yield return StartCoroutine(cEffector.UiOpacity2(text, 2, 0));
            yield return new WaitForSeconds(0.6f);
        }
        yield return StartCoroutine(SwitchMusic());
    }
    private IEnumerator SwitchMusic()
    {
        yield return StartCoroutine(Globals.audioManager.FadeOutC("menuTheme", 1));
        Globals.GoToScene(sceneName);
    }
}
