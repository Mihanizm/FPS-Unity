using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : MonoBehaviour
{
    public float rate = 3;
    public int damage = 1;
    public Material material;
    public float dmgMult = 2f;
    public AudioClip shotSound;

    private AudioSource _audio;

    void Start()
    {
        _audio = GetComponent<AudioSource>();
    }

    void Update()
    {

    }

    public void Fire(Ray ray)
    {
        _audio.PlayOneShot(shotSound);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.transform.gameObject;
            BehaviourAI target = hitObject.GetComponent<BehaviourAI>();
            HeadAI targetHead = hitObject.GetComponent<HeadAI>();
            if (hitObject.tag == "Enemy" && target != null)
            {
                target.ReactionToHit(damage, transform.parent.position, hit);

                Collider[] hitColliders = Physics.OverlapSphere(hit.point, 5, 1 << LayerMask.NameToLayer("Enemy"));
                foreach (var col in hitColliders)
                {
                    BehaviourAI bAI = col.gameObject.GetComponent<BehaviourAI>();
                    if (bAI != null)
                    {
                        bAI.ReactionToAlarm(transform.parent.position);
                    }
                }
            }
            else if (hitObject.tag == "Enemy Head" && targetHead != null)
            {
                targetHead.ReactToHit(Mathf.FloorToInt(damage * dmgMult), Vector3.Normalize(hit.point - transform.parent.position));
                Collider[] hitColliders = Physics.OverlapSphere(hit.point, 5, 1 << LayerMask.NameToLayer("Enemy"));
                foreach (var col in hitColliders)
                {
                    BehaviourAI bAI = col.gameObject.GetComponent<BehaviourAI>();
                    if (bAI != null)
                    {
                        bAI.ReactionToAlarm(transform.parent.position);
                    }
                }
            }
            else
            {
                WallBehaviour targetWall = hitObject.GetComponent<WallBehaviour>();
                if (targetWall != null)
                {
                    targetWall.ReactToHit(hit);
                }
            }
        }
    }

}
