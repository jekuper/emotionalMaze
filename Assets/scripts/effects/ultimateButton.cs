using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ultimateButton : MonoBehaviour
{
    private RectTransform tr;
    private bool isDestroying = false;
    void Start()
    {
        tr = GetComponent<RectTransform>();
        Vector3 targetPos = tr.anchoredPosition;
        Vector3 pos = new Vector3(targetPos.x, -targetPos.y, targetPos.z);
        tr.anchoredPosition = pos;
        StartCoroutine(cEffector.MoveUiTowards2(tr, targetPos, 0.2f));
    }
    public void ultimate()
    {
        if (isDestroying)
            return;
        isDestroying = true;
        Globals.player.GetComponent<movements>().ultimate();
        StartCoroutine(Dissapear());
    }
    public IEnumerator Dissapear()
    {
        Vector2 targetPos = new Vector2(tr.anchoredPosition.x, -tr.anchoredPosition.y);
        yield return StartCoroutine(cEffector.MoveUiTowards2(tr, targetPos, 0.2f));
        Destroy(gameObject);
    }
}
