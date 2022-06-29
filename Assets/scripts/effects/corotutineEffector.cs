using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class cEffector
{
    #region light resizing

    public static IEnumerator resizeLight(UnityEngine.Experimental.Rendering.Universal.Light2D lightPoint, float targetOuterRadius, float speed, bool includeInnerRadius = false)
    {
        speed = Mathf.Abs(speed);
        if (lightPoint.pointLightOuterRadius > targetOuterRadius)
            speed = -speed;
        while (Mathf.Abs(lightPoint.pointLightOuterRadius - targetOuterRadius) > 0.1)
        {
            float newRadius = lightPoint.pointLightOuterRadius + speed * Time.fixedDeltaTime;
            lightPoint.pointLightOuterRadius = newRadius;
            if (includeInnerRadius)
                lightPoint.pointLightInnerRadius = newRadius;
            yield return new WaitForFixedUpdate();
        }
        lightPoint.pointLightOuterRadius = targetOuterRadius;
    }
    public static IEnumerator resizeLight2(UnityEngine.Experimental.Rendering.Universal.Light2D lightPoint, float targetOuterRadius, float timing, bool includeInnerRadius = false)
    {
        float speed = Mathf.Abs(lightPoint.pointLightOuterRadius - targetOuterRadius) / timing;

        speed = Mathf.Abs(speed);
        if (lightPoint.pointLightOuterRadius > targetOuterRadius)
            speed = -speed;
        while (Mathf.Abs(lightPoint.pointLightOuterRadius - targetOuterRadius) > 0.1)
        {
            float newRadius = lightPoint.pointLightOuterRadius + speed * Time.fixedDeltaTime;
            lightPoint.pointLightOuterRadius = newRadius;
            if (includeInnerRadius)
                lightPoint.pointLightInnerRadius = newRadius;
            yield return new WaitForFixedUpdate();
        }
        lightPoint.pointLightOuterRadius = targetOuterRadius;
    }
    #endregion

    #region ui moving
    public static IEnumerator MoveUiTowards(RectTransform tr, Vector3 pos, float speed)
    {
        while (Mathf.Abs(tr.anchoredPosition.x - pos.x) > 0.1f || Mathf.Abs(tr.anchoredPosition.y - pos.y) > 0.1f)
        {
            Vector3 newPos = Vector3.MoveTowards(tr.anchoredPosition, pos, Time.fixedDeltaTime * speed);
            tr.anchoredPosition = newPos;
            yield return new WaitForFixedUpdate(); 
        }
        tr.anchoredPosition = pos;
    }
    public static IEnumerator MoveUiTowards2(RectTransform tr, Vector3 pos, float timing)
    {
        float speed = Vector2.Distance(tr.anchoredPosition, pos) / timing;

        while (Mathf.Abs(tr.anchoredPosition.x - pos.x) > 0.1f || Mathf.Abs(tr.anchoredPosition.y - pos.y) > 0.1f)
        {
            Vector3 newPos = Vector3.MoveTowards(tr.anchoredPosition, pos, Time.fixedDeltaTime * speed);
            tr.anchoredPosition = newPos;
            yield return new WaitForFixedUpdate();
        }
        tr.anchoredPosition = pos;
    }
    #endregion

    #region ui image fadeout
    public static IEnumerator UiFadeOut(Image ui, float speed, bool finalDestroy = false)
    {
        if (ui.GetComponent<Button>() != null)
        {
            ui.GetComponent<Button>().interactable = false;
        }
        Button[] childrenButton = ui.GetComponentsInChildren<Button>();
        foreach (Button button in childrenButton)
        {
            button.interactable = false;
        }
        while (ui.color.a > 0.1f)
        {
            float newAlpha = ui.color.a + speed * Time.fixedDeltaTime;
            ui.color = new Color(ui.color.r, ui.color.g, ui.color.b, newAlpha);
            Image[] childrenImg = ui.GetComponentsInChildren<Image>();
            foreach (Image img in childrenImg)
            {
                img.color = new Color(img.color.r, img.color.g, img.color.b, newAlpha);
            }
            TextMeshProUGUI[] childrenText = ui.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI text in childrenText)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, newAlpha);
            }
            yield return new WaitForFixedUpdate();
        }
        ui.color = new Color(ui.color.r, ui.color.g, ui.color.b, 0);
        if (finalDestroy)
        {
            Object.Destroy(ui.gameObject);
        }
    }
    public static IEnumerator UiFadeOut2(Image ui, float timing, bool finalDestroy = false)
    {
        float speed = -ui.color.a / timing;
        if (ui.GetComponent<Button>() != null)
        {
            ui.GetComponent<Button>().interactable = false;
        }
        Button[] childrenButton = ui.GetComponentsInChildren<Button>();
        foreach (Button button in childrenButton)
        {
            button.interactable = false;
        }
        while (ui.color.a > 0.1f)
        {
            float newAlpha = ui.color.a + speed * Time.fixedDeltaTime;
            ui.color = new Color(ui.color.r, ui.color.g, ui.color.b, newAlpha);
            Image[] childrenImg = ui.GetComponentsInChildren<Image>();
            foreach (Image img in childrenImg)
            {
                img.color = new Color(img.color.r, img.color.g, img.color.b, newAlpha);
            }
            TextMeshProUGUI[] childrenText = ui.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI text in childrenText)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, newAlpha);
            }
            yield return new WaitForFixedUpdate();
        }
        ui.color = new Color(ui.color.r, ui.color.g, ui.color.b, 0);
        if (finalDestroy)
        {
            Object.Destroy(ui.gameObject);
        }
    }
    #endregion

    #region ui TMPro fade
    public static IEnumerator UiOpacity2(TextMeshProUGUI ui, float timing, float targetOpacity, bool finalDestroy = false)
    {
        targetOpacity = Mathf.Clamp(targetOpacity, 0, 1);
        float speed = Mathf.Abs(ui.color.a - targetOpacity) / timing;
        if (targetOpacity < ui.color.a)
            speed = -speed;
        while (Mathf.Abs(ui.color.a - targetOpacity) > 0.1f)
        {
            float newAlpha = ui.color.a + speed * Time.fixedDeltaTime;
            ui.color = new Color(ui.color.r, ui.color.g, ui.color.b, newAlpha);
            yield return new WaitForFixedUpdate();
        }
        ui.color = new Color(ui.color.r, ui.color.g, ui.color.b, targetOpacity);
        if (finalDestroy)
        {
            Object.Destroy(ui.gameObject);
        }
    }
    #endregion

    #region sprite scale
    public static IEnumerator SpriteScale2(Transform sprite, float timing, Vector3 targetScale)
    {
        float speed = Vector3.Distance(targetScale, sprite.localScale) / timing;
        while (Vector3.Distance(targetScale, sprite.localScale) > 0.1f)
        {
            sprite.localScale = Vector3.MoveTowards(sprite.localScale, targetScale, speed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
        sprite.localScale = targetScale;
    }
    #endregion
}
