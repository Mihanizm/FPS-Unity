using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayShooter : MonoBehaviour
{
	public int damage = 1;

	private Camera _camera;
	private GameObject _gun;

    void Start()
    {
        _camera = GetComponent<Camera>();
		_gun = transform.Find("Gun").gameObject;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
    }
	
	void OnGUI()
	{
			int size = 12;
			float posX = _camera.pixelWidth/2 - size/4;
			float posY = _camera.pixelHeight/2 - size/2;
			GUI.Label(new Rect(posX,posY,size,size), "*");
	}

    void Update()
    {
        if (Input.GetMouseButton(0))
		{
			Vector3 point = new Vector3(_camera.pixelWidth/2,_camera.pixelHeight/2,0);
			Ray ray = _camera.ScreenPointToRay(point);
			GunBehaviour gB = _gun.GetComponent<GunBehaviour>();
			if (gB != null)
			{
				gB.Fire(ray);
			}
		}

		float mI = Input.GetAxis("Mouse ScrollWheel");
		if (mI != 0)
		{
			GunBehaviour gB = _gun.GetComponent<GunBehaviour>();
			if (gB != null)
			{
				gB.ChangeGunType(mI);
			}
		}
    }
}
