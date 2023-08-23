using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadAI : MonoBehaviour
{
    public enum StatusHead { Hold, Up, Down, Dead }
    public StatusHead _currentStatusHead = StatusHead.Hold;

    public float maxHeight = 1;
    public float speed = 1;
    public float minDelay = 0.5f;
    public float maxDelay = 3;

    private float _delay;
    private float _currentDelay;
    private Vector3 _moveDirection;
    private BehaviourAI _parentBAI;

    void Start()
    {
        _moveDirection = Vector3.zero;
        _currentDelay = 0;
        _delay = 0.1f;
        _parentBAI = transform.parent.gameObject.GetComponent<BehaviourAI>();
    }
    
    void Update()
    {
        Move();
    }

    private void Move()
    {
        if (_currentStatusHead != StatusHead.Dead)
        {
            if (_currentStatusHead == StatusHead.Hold)
            {
                Hold();
            }
            else if (_currentStatusHead == StatusHead.Up)
            {
                Up();
            }
            else if (_currentStatusHead == StatusHead.Down)
            {
                Down();
            }

            transform.Translate(_moveDirection * speed * Time.deltaTime);
        }
    }

    private void Hold()
    {
        if (_currentDelay > _delay)
        {
            _currentStatusHead = StatusHead.Up;
            _moveDirection = transform.up;
            _currentDelay = 0;
            _delay = Random.Range(minDelay, maxDelay);
        }
        else
        {
            _currentDelay += Time.deltaTime;
            _moveDirection = Vector3.zero;
        }
    }

    private void Up()
    {
        if (transform.localPosition.y > maxHeight)
        {
            _currentStatusHead = StatusHead.Down;
            _moveDirection = -transform.up;
        }
    }

    private void Down()
    {
        if (transform.localPosition.y < 0)
        {
            _currentStatusHead = StatusHead.Hold;
            _moveDirection = Vector3.zero;
        }
    }

    public void Dead(Vector3 vectorHit)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        GetComponent<Collider>().isTrigger = false;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.AddForce(vectorHit * 1, ForceMode.Impulse);
        StartCoroutine(_parentBAI.Die());
    }

    public void ReactToHit(int damage, Vector3 vectorHit)
    {
        if (_parentBAI != null)
        {
            _parentBAI.hitPoint -= 2*damage;

            if (_parentBAI.hitPoint <= 0)
            {
                _currentStatusHead = StatusHead.Dead;
                Dead(vectorHit);
            }
        }
    }
}
