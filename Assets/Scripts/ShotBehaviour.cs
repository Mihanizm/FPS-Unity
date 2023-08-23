using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DecalSystem;

public class ShotBehaviour : MonoBehaviour
{
    private Decal _decal;

    void Start()
    {
        CreatePrefab();
    }

    void Update()
    {
        
    }

    void CreatePrefab()
    {
        //GameObject obj = Instantiate(_decal, transform.position, _decal.transform.rotation);

        //decal implementation!!
        _decal = GetComponent<Decal>();
        if (_decal) //if this obj has decal script
        {
            var filter = _decal.GetComponent<MeshFilter>();
            var mesh = filter.mesh;
            if (mesh != null)
            {
                mesh.name = "DecalMesh";
                filter.mesh = mesh;
            }
            DecalBuilder.Build(_decal);
        }
    }
}
