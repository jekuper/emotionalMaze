using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class sceneLoadEffect : MonoBehaviour
{
    public UnityEngine.Experimental.Rendering.Universal.Light2D lightP;
    public GameObject particle;
    public RectTransform target;

    public GameObject[] toDelete;

    private bool isLoading = false;
    private void Start()
    {
        Vector3 uiPos = target.TransformPoint(target.rect.center);
        transform.position = new Vector3(uiPos.x, uiPos.y, transform.position.z); 
    }
    public void StartMenuEffect () {
        if (!Globals.isFirstLoad) {
            Globals.audioManager.Stop ("menuTheme");
            StartEffect ("main");
        }
        else
            StartEffect ("prequil");
    }
    public void StartEffect(string sceneName)
    {
        if (!isLoading)
        {
            isLoading = true;
            StartCoroutine(Effect(sceneName));
        }
    }
    private IEnumerator Effect(string sceneName)
    {
//        Instantiate(particle, transform.position, new Quaternion());
        
        foreach (GameObject item in toDelete)
        {
            StartCoroutine(cEffector.UiFadeOut2(item.GetComponent<Image>(), 1f, true));
        }
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(cEffector.resizeLight2(lightP, 30, 5f));
        yield return StartCoroutine(cEffector.resizeLight2(lightP, 0, 1.5f));
        Globals.GoToScene(sceneName);
    }
    
    
    /*
    private IEnumerator ResizeLight(Vector2 targetSize)
    {
        float newX = lightSprite.transform.localScale.x, newY = lightSprite.transform.localScale.y;
        while(Mathf.Abs(newX - targetSize.x) > 0.2 || Mathf.Abs(newY - targetSize.y) > 0.2)
        {
            newX = Mathf.Lerp(newX, targetSize.x, 0.9f * Time.fixedDeltaTime);
            newY = Mathf.Lerp(newY, targetSize.y, 0.9f * Time.fixedDeltaTime);
            lightSprite.transform.localScale = new Vector3(newX, newY, lightSprite.transform.localScale.z);
            yield return new WaitForFixedUpdate();
        }
        lightSprite.transform.localScale = new Vector3(targetSize.x, targetSize.y, lightSprite.transform.localScale.z);
    }
    */
}
