using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyBurst : MonoBehaviour
{
    void Start()
    {
        cameraShake cam = FindObjectOfType<cameraShake>();
        StartCoroutine(cam.Shake(0.5f, 0.45f));
    }

}
