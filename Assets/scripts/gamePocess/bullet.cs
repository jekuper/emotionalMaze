using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public float speed;

    private Rigidbody2D rb;
    private Vector2 direction;
    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        direction = transform.rotation * Vector2.up;
    }
    private void FixedUpdate() {
        rb.MovePosition((Vector2)transform.position + direction * Time.deltaTime * speed);
    }
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.transform.tag != "enemy" && collision.transform.tag != "Player")
            Destroy(gameObject);
    }
}
