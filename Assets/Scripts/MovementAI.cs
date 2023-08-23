using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAI : MonoBehaviour
{
    Vector3 target;

    void Start()
    {
        target = new Vector3(1, 0, 1);
    }

    void Update()
    {
        if (Dist(transform.forward.normalized, target.normalized) > 0.05f)
        {
            transform.forward = Vector3.MoveTowards(transform.forward, target, 3 * Time.deltaTime);
        }
    }

    float Dist(Vector3 v1, Vector3 v2)
    {
        float res = 0;
        res = Mathf.Abs(v1.x - v2.x) + Mathf.Abs(v1.z - v2.z);
        return res;
    }

    public void React(Vector3 origin)
    {
        target = new Vector3(origin.x, transform.position.y, origin.z) - transform.position;
        Debug.Log(origin);
    }
}
