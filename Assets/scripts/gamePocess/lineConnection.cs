using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lineConnection : MonoBehaviour
{
    public Transform target1, target2;
    public Gradient gradient;
    
    private DistanceJoint2D joint;
    private SpriteRenderer sr;
    private void Start() {
        sr = GetComponent<SpriteRenderer>();
        if (target1.GetComponent<DistanceJoint2D>() != null) {
            joint = target1.GetComponent<DistanceJoint2D>();
        }
        else {
            joint = target2.GetComponent<DistanceJoint2D>();
        }
    }
    void LateUpdate()
    {
        sr.size = new Vector2(sr.size.x, Vector2.Distance(target1.position, target2.position));
        transform.position = (Vector2)(target2.position);
        Vector3 dir = (target1.position - target2.position).normalized;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, Globals.vectorToAngle(dir) - 90));
        Vector2[] points = new Vector2[2];
        points[0] = target1.position;
        points[1] = target2.position;
        if (target1.GetComponent<movements>().curStage == 1)
            sr.color = gradient.Evaluate(Vector2.Distance(target1.position, target2.position) / joint.distance);
        else
            sr.color = Color.white;
    }
}
