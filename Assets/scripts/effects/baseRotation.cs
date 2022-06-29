using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class baseRotation : MonoBehaviour
{
    public Transform secondCircle; 
    public float rotSpeed1, rotSpeed2;

    private void Start()
    {
    }
    private void FixedUpdate()
    {
        transform.Rotate(0, 0, rotSpeed1 * Time.deltaTime);
        if(secondCircle != null)
        {   
            secondCircle.Rotate(0, 0, (rotSpeed2 - rotSpeed1) * Time.deltaTime);
        }
    }
}
