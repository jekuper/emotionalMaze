using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetingEnemy : MonoBehaviour
{
    public Transform target;
    public GameObject bulletPrefab;
    public GameObject burstEffect;
    public Rigidbody2D rb;
    public Transform bulletSpawnPoint;
    public float timeoutInit = 3f;
    public int stage = 1;
    public float movingSpeed = 3f;

    private float timeout;
    private void Start() {
        timeout = timeoutInit;
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        Vector2 dir = (target.position - transform.position).normalized;
        float angle = Globals.vectorToAngle(dir) - 90;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        if (timeout <= Time.deltaTime) {
            fire();
            timeout = timeout + timeoutInit - Time.deltaTime;
        }
        else {
            timeout -= Time.deltaTime;
        }
        if (stage == 2)
        {
            Vector2 move = (target.position - transform.position).normalized;
            rb.MovePosition((Vector2)transform.position + move * Time.deltaTime * movingSpeed);
        }
    }
    public void fire() {
        float angle = transform.rotation.eulerAngles.z;
        GameObject bulletClone = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.Euler(0, 0, angle));
        bulletClone.transform.localScale = new Vector3(transform.localScale.x / 5.2f, transform.localScale.y / 5.2f, 1);
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (stage == 2 && collider.tag == "chain")
        {
            target.GetComponent<movements>().found++;
            target.GetComponent<movements>().UpdateScore();
            Instantiate(burstEffect, transform.position, new Quaternion());
            Destroy(gameObject);
        }
    }
}
