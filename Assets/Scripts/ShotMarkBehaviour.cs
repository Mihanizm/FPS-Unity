using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotMarkBehaviour : MonoBehaviour
{
    public float timeLife;

    void Start()
    {
        StartCoroutine(Die());
    }

    
    void Update()
    {
        
    }

    private IEnumerator Die()
    {
        yield return new WaitForSeconds(timeLife);

        Destroy(this.gameObject);
    }
}
