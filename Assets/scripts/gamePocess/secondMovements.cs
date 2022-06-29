using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class secondMovements : MonoBehaviour
{
    public float speed = 3f;
    public Joystick joystick;
    public GameObject secondCircle;

    private Rigidbody2D rb;
    void Start()
    {
        joystick.enabled = true;
        Color white = new Color(1, 1, 1, joystick.GetComponent<Image>().color.a);
        joystick.GetComponent<Image>().color = white;
        joystick.gameObject.transform.Find("Handle").GetComponent<Image>().color = white;
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        Destroy(secondCircle);
    }
    void FixedUpdate()
    {
        Vector2 move = Vector2.zero;
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            move = new Vector2(joystick.Horizontal, joystick.Vertical);
        }
        else
        {
            move = new Vector2(Input.GetAxis("Horizontal2"), Input.GetAxis("Vertical2"));
        }
        rb.MovePosition((Vector2)transform.position + move * Time.deltaTime * speed);
    }
}
