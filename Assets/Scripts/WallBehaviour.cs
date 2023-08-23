using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBehaviour : MonoBehaviour
{
    public GameObject shotMark;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void ReactToHit(RaycastHit hit)
    {
        Instantiate(shotMark, hit.point + hit.normal * 0.0001f, Quaternion.FromToRotation(Vector3.up, hit.normal));
    }
}
