using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrowManager : MonoBehaviour
{
    public Transform pin = null, target = null;
    public bool isStartTarget = false;
    public Sprite keySprite, starSprite;

    private SpriteRenderer sr;
    private void OnEnable()
    {
        sr = GetComponent<SpriteRenderer>();
        if (isStartTarget)
            sr.sprite = starSprite;
        else
            sr.sprite = keySprite;
    }
    private void Update() {
        if (pin != null && target != null) {
            Vector3 pos = new Vector3(pin.position.x, pin.position.y, transform.position.z);
            Vector3 direction = (target.position - pin.position).normalized;
            pos += direction * 1.5f;
            transform.position = pos;
            float angle = Vector2.Angle(direction, Vector2.up);
            if(direction.x > 0) {
                angle = 360 - angle;
            }
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
