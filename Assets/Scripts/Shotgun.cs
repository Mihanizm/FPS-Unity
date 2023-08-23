using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    public float rate = 1;
    public int damage = 1;
    public int countBullet = 10;
    public Material material;
    public float dmgMult = 3f;
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

        for (int i = 0; i < countBullet; i++)
        {
            Vector3 spread = Vector3.zero;
            Transform tp = transform.parent;

            spread += tp.up * UnityEngine.Random.Range(-1f, 1f);
            spread += tp.right * UnityEngine.Random.Range(-1f, 1f);
            ray.direction += spread.normalized * UnityEngine.Random.Range(0f, 0.1f);

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

}
