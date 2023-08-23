using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactiveTarget : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public void ReactToHit(Vector3 vectorHit)
	{
		WanderingAI behavior = GetComponent<WanderingAI>();
		if (behavior != null)
			behavior.SetAlive(1);
		
		Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
		rb.constraints = RigidbodyConstraints.None;
		rb.AddForce(vectorHit * 5, ForceMode.Impulse);
		
		StartCoroutine(Die());
	}
	
	private IEnumerator Die()
	{
		//transform.Rotate(-75,0,0);
		
		yield return new WaitForSeconds(1.5f);
		
		Destroy(gameObject);
	}
}
