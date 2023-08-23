using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourAI : MonoBehaviour
{
    public float speed = 7.0f;
    public float rangeObstacle = 2.0f;
    public float rangePlayer = 10.0f;
    public int hitPoint = 3;
    public float rateOfFire = 1;
    public GameObject shotMark;
    public GameObject bullet;
    public AudioClip shotSound;

    private enum Status { Calm, Rage, Dead }
    private enum StatusMoveCalm { Patrol, Rotate }
    private enum StatusMoveRage { Approach, Hold, Retreat }

    private float minRangePlayer;
    private float maxRangePlayer;
    private int xDir;
    private Vector3 targetRotate;

    [SerializeField] private Material _calmMaterial;
    [SerializeField] private Material _rageMaterial;
    [SerializeField] private Material _deadMaterial;
    private MeshRenderer _meshRenderer;
    private Status _currentStatus;
    private StatusMoveCalm _currentStatusMoveCalm;
    private StatusMoveRage _currentStatusMoveRage;
    private GameObject _player;
    private Vector3 _movementDirection;
    private GameObject _gun;
    private float _currentRateOfFire;
    private AudioSource _audio;

    void Start()
    {
        _currentStatus = Status.Calm;
        _meshRenderer = GetComponent<MeshRenderer>();
        minRangePlayer = 0.85f * rangePlayer;
        maxRangePlayer = 1.15f * rangePlayer;
        _player = GameObject.FindGameObjectWithTag("Player");
        _gun = transform.Find("Enemy Gun").gameObject;
        _currentRateOfFire = 0;
        _audio = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (_currentStatus != Status.Dead)
        {
            Look();
            Move();

            if (_currentRateOfFire > 0)
            {
                _currentRateOfFire -= Time.deltaTime;
            }
        }
    }

    private void Look()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hitLook;

        if (Physics.SphereCast(ray, 0.2f, out hitLook))
        {
            GameObject hitObjectLook = hitLook.transform.gameObject;
            if (hitObjectLook.tag == "Player")
            {
                Attack(hitLook.distance);
            }
            else if (hitObjectLook.tag == "Enemy" || hitObjectLook.tag == "Enemy Head")
            {

            }
            else if (hitObjectLook.tag == "Wall")
            {
                if (_currentStatus == Status.Rage)
                {
                    _currentStatusMoveCalm = StatusMoveCalm.Patrol;
                }
                _currentStatus = Status.Calm;
                _meshRenderer.material = _calmMaterial;
            }
        }
    }

    private void Move()
    {
        if (_currentStatus == Status.Calm)
        {
            if (_currentStatusMoveCalm == StatusMoveCalm.Patrol)
            {
                MovePatrol();
            }
            else if (_currentStatusMoveCalm == StatusMoveCalm.Rotate)
            {
                MoveRotate();
            }
        }
        else if (_currentStatus == Status.Rage)
        {
            transform.LookAt(new Vector3(_player.transform.position.x,transform.position.y,_player.transform.position.z));

            if (_currentStatusMoveRage == StatusMoveRage.Approach)
            {
                MoveApproach();
            }
            else if (_currentStatusMoveRage == StatusMoveRage.Hold)
            {
                MoveHold();
            }
            else
            {
                MoveRetreat();
            }
        }

        transform.Translate(_movementDirection);
    }

    void Attack(float distance)
    {
        if (_currentStatus == Status.Calm)
        {
            xDir = Random.Range(0, 2) == 0 ? -1 : 1;
            _currentStatus = Status.Rage;
            _meshRenderer.material = _rageMaterial;
        }

        _currentStatusMoveRage = GetStatusMoveRage(distance);

        if (_currentRateOfFire <= 0)
        {
            Vector3 startPosBullet = _gun.transform.position + _gun.transform.forward * _gun.transform.localScale.z;
            Instantiate(bullet, startPosBullet, _gun.transform.rotation);
            _audio.PlayOneShot(shotSound);
            _currentRateOfFire = rateOfFire;
        }
    }

    void MovePatrol()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 3 * rangeObstacle, 1 << LayerMask.NameToLayer("Player"));
        if (hitColliders.Length > 0)
        {
            Vector3 targetPos = hitColliders[0].transform.position;
            targetRotate = new Vector3(targetPos.x,transform.position.y,targetPos.z)-transform.position;
            _currentStatusMoveCalm = StatusMoveCalm.Rotate;
            _movementDirection = new Vector3(0, 0, 0);
        }
        else
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hitObstacle;

            if (Physics.SphereCast(ray, 0.75f, out hitObstacle))
            {
                GameObject hitObjectObstacle = hitObstacle.transform.gameObject;

                if ((hitObjectObstacle.tag == "Wall" || hitObjectObstacle.tag == "Enemy" || hitObjectObstacle.tag == "Enemy Head") && hitObstacle.distance < rangeObstacle)
                {
                    _currentStatusMoveCalm = StatusMoveCalm.Rotate;
                    targetRotate = Quaternion.Euler(0, Random.Range(-110, 110), 0) * transform.forward;
                    _movementDirection = new Vector3(0, 0, 0);
                }
                else
                {
                    _movementDirection = new Vector3(0, 0, speed * Time.deltaTime);
                }
            }
        }
    }

    void MoveRotate()
    {
        transform.forward = Vector3.MoveTowards(transform.forward, targetRotate, 5f * Time.deltaTime);
        Debug.DrawRay(transform.position, targetRotate);
        if (Similarity(transform.forward.normalized, targetRotate.normalized) < 0.01f)
        {
            _currentStatusMoveCalm = StatusMoveCalm.Patrol;
        }
    }

    void MoveApproach()
    {
        RaycastHit hitObstacle;
        _movementDirection = new Vector3(0, 0, speed * Time.deltaTime);

        if (Physics.SphereCast(transform.position, 0.75f, transform.forward, out hitObstacle))
        {
            GameObject hitObstacleObject = hitObstacle.transform.gameObject;
            if ((hitObstacleObject.tag == "Wall" || hitObstacleObject.tag == "Enemy" || hitObstacleObject.tag == "Enemy Head")
                && hitObstacle.distance < rangeObstacle)
            {
                _movementDirection = new Vector3(0, 0, 0);
            }
        }
    }

    void MoveHold()
    {
        RaycastHit hitObstacleLeft;
        RaycastHit hitObstacleRight;

        if (Physics.SphereCast(transform.position, 0.5f, Quaternion.Euler(0, -90, 0) * transform.forward, out hitObstacleLeft)
            && Physics.SphereCast(transform.position, 0.5f, Quaternion.Euler(0, 90, 0) * transform.forward, out hitObstacleRight))
        {
            GameObject hitObstacleLeftObject = hitObstacleLeft.transform.gameObject;
            GameObject hitObstacleRightObject = hitObstacleRight.transform.gameObject;

            if ((hitObstacleLeftObject.tag == "Wall" || hitObstacleLeftObject.tag == "Enemy" || hitObstacleLeftObject.tag == "Enemy Head")
                && hitObstacleLeft.distance < rangeObstacle)
            {
                xDir = 1;
            }
            else if ((hitObstacleRightObject.tag == "Wall" || hitObstacleRightObject.tag == "Enemy" || hitObstacleRightObject.tag == "Enemy Head")
                && hitObstacleRight.distance < rangeObstacle)
            {
                xDir = -1;
            }
        }
        _movementDirection = new Vector3(xDir * speed / 2.0f * Time.deltaTime, 0, 0);
    }

    void MoveRetreat()
    {
        RaycastHit hitObstacleBack;
        _movementDirection = new Vector3(0, 0, -speed * Time.deltaTime);

        if (Physics.SphereCast(transform.position, 0.5f, -transform.forward, out hitObstacleBack))
        {
            if (hitObstacleBack.transform.gameObject.tag == "Wall" && hitObstacleBack.distance < rangeObstacle)
            {
                _movementDirection = new Vector3(0, 0, 0);
            }
        }
    }

    float Similarity(Vector3 v1, Vector3 v2)
    {
        return Mathf.Abs(v1.x - v2.x) + Mathf.Abs(v1.z - v2.z);
    }

    private StatusMoveRage GetStatusMoveRage(float distance)
    {
        if (distance > maxRangePlayer)
        {
            return StatusMoveRage.Approach;
        }
        else if (distance <= maxRangePlayer && distance >= minRangePlayer)
        {
            return StatusMoveRage.Hold;
        }
        else
        {
            return StatusMoveRage.Retreat;
        }
    }

    public void ReactionToHit(int damage, Vector3 origin, RaycastHit hit)
    {
        Instantiate(shotMark, hit.point + hit.normal * 0.0001f, Quaternion.FromToRotation(Vector3.up, hit.normal), hit.transform);
        Hurt(damage, Vector3.Normalize(hit.point - origin));
    }

    public void ReactionToHit(int damage, Vector3 vectorHit)
    {
        Hurt(damage, vectorHit);
    }

    public void ReactionToAlarm(Vector3 originHit)
    {
        targetRotate = new Vector3(originHit.x, transform.position.y, originHit.z) - transform.position;
        _currentStatusMoveCalm = StatusMoveCalm.Rotate;
        _movementDirection = new Vector3(0, 0, 0);
    }

    private void Hurt(int damage, Vector3 vectorHit)
    {
        hitPoint -= damage;

        if (hitPoint <= 0)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb = GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.AddForce(vectorHit * 10, ForceMode.Impulse);

            HeadAI hAI = transform.Find("Head").gameObject.GetComponent<HeadAI>();
            if (hAI != null)
            {
                hAI.Dead(vectorHit);
            }

            StartCoroutine(Die());
        }
    }

    public IEnumerator Die()
    {
        _currentStatus = Status.Dead;
        _meshRenderer.material = _deadMaterial;
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }
}
