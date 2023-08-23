using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GunBehaviour : MonoBehaviour
{
    public enum GunType { Pistol, Rifle, Shotgun };
    public GunType _currentGunType = GunType.Pistol;
    private int lenGunType;

    private float _currentRateOfFire;
    private Ray _currentRay;
    private Pistol _pistol;
    private Rifle _rifle;
    private Shotgun _shotgun;
    private MeshRenderer _meshRenderer;
    void Start()
    {
        _currentRateOfFire = 0;
        lenGunType = Enum.GetNames(typeof(GunType)).Length;
        _pistol = GetComponent<Pistol>();
        _rifle = GetComponent<Rifle>();
        _shotgun = GetComponent<Shotgun>();
        _meshRenderer = GetComponent<MeshRenderer>();

        CheckGunMaterial();
    }

    void Update()
    {
        if (_currentRateOfFire > 0)
        {
            _currentRateOfFire -= Time.deltaTime;
        }
    }

    public void ChangeGunType(float valScroll)
    {
        if (valScroll < 0)
        {
            ChangeGunTypeDown();
        }
        else if (valScroll > 0)
        {
            ChangeGunTypeUp();
        }
        _currentRateOfFire = 0;

        CheckGunMaterial();
    }

    private void ChangeGunTypeDown()
    {
        if (((int)_currentGunType) == lenGunType-1)
        {
            _currentGunType = (GunType)(0);
        }
        else
        {
            _currentGunType++;
        }
    }

    private void ChangeGunTypeUp()
    {
        if (((int)_currentGunType) == 0)
        {
            _currentGunType = (GunType)(lenGunType - 1);
        }
        else
        {
            _currentGunType--;
        }
    }

    public void Fire(Ray ray)
    {
        _currentRay = ray;

        if (_currentRateOfFire <= 0)
        {
            if (_currentGunType == GunType.Pistol)
            {
                _currentRateOfFire = 1/_pistol.rate;
                _pistol.Fire(ray);
            }
            else if (_currentGunType == GunType.Rifle)
            {
                _currentRateOfFire = 1/_rifle.rate;
                _rifle.Fire(ray);
            }
            else if (_currentGunType == GunType.Shotgun)
            {
                _currentRateOfFire = 1/_shotgun.rate;
                _shotgun.Fire(ray);
            }
        }
    }

    private void CheckGunMaterial()
    {
        if (_currentGunType == GunType.Pistol)
        {
            _meshRenderer.material = _pistol.material;
        }
        else if (_currentGunType == GunType.Rifle)
        {
            _meshRenderer.material = _rifle.material;
        }
        else if (_currentGunType == GunType.Shotgun)
        {
            _meshRenderer.material = _shotgun.material;
        }
    }
}
