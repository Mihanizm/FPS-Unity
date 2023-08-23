using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20;
    public int damage = 1;

    private Vector3 startPoint;

    void Start()
    {
        startPoint = transform.position;
    }

    void Update()
    {
        transform.Translate(0, 0, speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerCharacter pc = other.GetComponent<PlayerCharacter>();
            if (pc != null)
            {
                pc.Hurt(damage);
            }
        }
        else if (other.tag == "Enemy")
        {
            BehaviourAI bAI = other.GetComponent<BehaviourAI>();
            if (bAI != null)
            {
                Vector3 vectorHit = other.transform.position - startPoint;
                bAI.ReactionToHit(damage, vectorHit.normalized);
            }
        }

        Destroy(this.gameObject);
    }
}
