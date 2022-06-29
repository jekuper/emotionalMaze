using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class cameraMovement : MonoBehaviour
{
    public Transform target;
    public Camera cam;

    public Transform rightTop;
    public Transform leftBottom;


    [Range(0.01f, 1f)] public float smoothing;

    private void FixedUpdate() {
        if (leftBottom != null && rightTop != null) {
            Vector3 tarPos = target.position;
            Vector3 myPos = transform.position;
            Vector3 TR = rightTop != null ? rightTop.position : Vector3.positiveInfinity;
            Vector3 BL = leftBottom != null ? leftBottom.position : Vector3.negativeInfinity;

            float newX = Mathf.Lerp(myPos.x, tarPos.x, smoothing);
            float newY = Mathf.Lerp(myPos.y, tarPos.y, smoothing);

            float xK = (float)Screen.width / (float)Screen.height;
            newX = Mathf.Clamp(newX, BL.x + cam.orthographicSize * xK, TR.x - cam.orthographicSize * xK);
            newY = Mathf.Clamp(newY, BL.y + cam.orthographicSize, TR.y - cam.orthographicSize);

            transform.position = new Vector3(newX, newY, myPos.z);
        }
    }

    
}
