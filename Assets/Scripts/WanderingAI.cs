using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderingAI : MonoBehaviour
{
	[SerializeField] private GameObject fireballPrefab;
	[SerializeField] private Material _calmMaterial;
	[SerializeField] private Material _rageMaterial;
	private GameObject _fireball;
	private GameObject _currentTarget;
	private int _alive;
	private Vector3 _moveDirection;
	private float _moveX;
	private float _moveY;
	private float _moveZ;
	
	public float speed = 3.0f;
	public float obstacleRange = 3.0f;
	public float obstacleBackRange = 1.0f;
	public float enemyRange = 5.0f;
	public float playerRange = 10.0f;
	
	public enum Status
	{
		Calm = 0,
		Rage = 1
	}
	
	private Status currentStatus;
	
    void Start()
    {
        _alive = 0;
		_currentTarget = null;
		_moveX = 0;
		_moveY = 0;
		_moveZ = speed * Time.deltaTime;
		_moveDirection = new Vector3(_moveX,_moveY,_moveZ);
		
		currentStatus = Status.Calm;
    }

    void Update()
    {
		if (_alive == 0)
		{
			if (currentStatus == Status.Rage)
			{
				transform.LookAt(new Vector3(_currentTarget.transform.position.x,transform.position.y,_currentTarget.transform.position.z));
			}
			
			Ray ray = new Ray(transform.position,transform.forward);
			
			RaycastHit hitObstacle;
			RaycastHit hitObstacleBack;
			RaycastHit hitPlayer;

			if (Physics.SphereCast(ray, 0.05f, out hitPlayer) && 
				Physics.SphereCast(ray, 0.75f, out hitObstacle) &&
				Physics.BoxCast(transform.position, transform.localScale*1.1f/2, -transform.forward, out hitObstacleBack))
			{
				GameObject hitObjectObstacle = hitObstacle.transform.gameObject;
				GameObject hitObjectObstacleBack = hitObstacleBack.transform.gameObject;
				GameObject hitObjectPlayer = hitPlayer.transform.gameObject;
				
				if (hitObjectPlayer.tag == "Player")
				{
					Fire();
					
					currentStatus = Status.Rage;
					GetComponent<MeshRenderer>().material = _rageMaterial;
					_currentTarget = hitPlayer.transform.gameObject;
				
					if (hitPlayer.distance < playerRange)
					{
						_moveZ = -speed * Time.deltaTime;
					}
					else
					{
						_moveZ = speed * Time.deltaTime;
					}
				}
				else if (hitObjectPlayer.tag == "Wall")
				{
					currentStatus = Status.Calm;
					GetComponent<MeshRenderer>().material = _calmMaterial;
				}
				
				if (hitObjectObstacle.tag == "Wall" || hitObjectObstacle.tag == "Enemy")
				{
					if (hitObstacle.distance <= obstacleRange)
					{				
						if (currentStatus == Status.Rage)
						{
							_moveZ = 0;
						}
						else if (currentStatus == Status.Calm)
						{
							float angle = Random.Range(-110,110);
							transform.Rotate(0,angle,0);
						}
					}
				}
				
				if (hitObstacleBack.distance <= obstacleBackRange && (hitObjectObstacleBack.tag == "Wall" || hitObjectObstacleBack.tag == "Enemy"))
				{
					if (currentStatus == Status.Rage)
					{
						_moveZ += speed * Time.deltaTime;
					}
				}
			}
			_moveDirection = new Vector3(_moveX,_moveY,_moveZ);
			transform.Translate(_moveDirection.x,_moveDirection.y,_moveDirection.z);
			
			_moveZ = speed * Time.deltaTime;
		}
    }
	
	public void SetAlive(int alive)
	{
		_alive = alive;
	}
	
	void Fire()
	{
		if (_fireball == null)
		{
			_fireball = Instantiate(fireballPrefab) as GameObject;
			_fireball.transform.position = transform.TransformPoint(Vector3.forward * 1.5f);
			_fireball.transform.rotation = transform.rotation;
		}
	}
}
