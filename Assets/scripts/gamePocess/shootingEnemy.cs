using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shootingEnemy : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float timeoutInit;
    public List<Vector2> positions;
    public float movingSpeed = 3f;
    public float rotSpeed = 0.5f;
    public int curStage = 1;
    public Transform target;
    public Transform bulletSpawnPoint;

    private float timeout;
    private int ind = 1;
    private int indIncrement = 1;
    private float EPS = 0.05f;
    private Rigidbody2D rb;
    private bool isRotating = true;
    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }
    
    private void FixedUpdate() {
        if (!isRotating) {
            if (timeout <= Time.deltaTime) {
                fire();
                timeout = timeout + timeoutInit - Time.deltaTime;
            }
            else {
                timeout -= Time.deltaTime;
            }
        }
        if(positions.Count > 1) {
            if (!isRotating) {
                if (Mathf.Abs(transform.position.x - positions[ind].x) < EPS && Mathf.Abs(transform.position.y - positions[ind].y) < EPS) {
                    transform.position = positions[ind];
                    if (ind == positions.Count - 1) {
                        indIncrement = -1;
                    }
                    else if (ind == 0) {
                        indIncrement = 1;
                    }
                    ind += indIncrement;
                    isRotating = true;
                }
                else {
                    Vector2 move = (positions[ind] - (Vector2)transform.position).normalized;
                    rb.MovePosition((Vector2)transform.position + move * Time.deltaTime * movingSpeed);
                }
            }
            else {
                Vector2 dir = (positions[ind] - (Vector2)transform.position).normalized;
                float angle = Globals.vectorToAngle(dir) - 90;
                if(Mathf.Abs(Globals.angleToPostive(transform.rotation.eulerAngles.z) - Globals.angleToPostive(angle)) < 1) {
                    transform.rotation = Quaternion.Euler(0, 0, angle);
                    isRotating = false;
                }
                else {
                    Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotSpeed * Time.deltaTime);
                    
                }
            }
        }
        else {
            isRotating = false;
        }
    }
    public void fire() {
        return;
        //Vector2 dir = (positions[ind] - (Vector2)transform.position).normalized;
        //float angle = Globals.vectorToAngle(dir);
        float angle = transform.rotation.eulerAngles.z;
        GameObject bulletClone = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.Euler(0, 0, angle));
        bulletClone.transform.localScale = new Vector3(transform.localScale.x / 5.2f, transform.localScale.y / 5.2f, 1);
    }
}
