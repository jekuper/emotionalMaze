using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class joystickSwap : MonoBehaviour
{
    public RectTransform joystick1, joystick2;
    private void Start()
    {
        if (Globals.mainSettings.swapJoysticks)
        {
            Vector3 pos1 = joystick1.position;
            Vector3 pos2 = joystick2.position;
            joystick1.position = pos2;
            joystick2.position = pos1;
        }
    }
}
