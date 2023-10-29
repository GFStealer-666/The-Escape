using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TestInvisibleWall : NetworkBehaviour
{
    private MeshRenderer _invisbleWallMeshRenderer;

    private BoxCollider _invisibleWallBoxCollider;

    [SerializeField] private bool _deWallCollider = false;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {   
             _invisbleWallMeshRenderer = child.GetComponent<MeshRenderer>();

            if ( _invisbleWallMeshRenderer != null)
            {
                _invisbleWallMeshRenderer.enabled = false;
            }
        }

    }
    void Update()
    {
        if(_deWallCollider == true){
            foreach(Transform child in transform){
                _invisibleWallBoxCollider = child.GetComponent<BoxCollider>();

                if(_invisibleWallBoxCollider != null)
                {
                    _invisibleWallBoxCollider.enabled = false;
                }
            }
        }
    }
    // Update is called once per frame
    
}
