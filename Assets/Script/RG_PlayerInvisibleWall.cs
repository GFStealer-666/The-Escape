using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class RG_PlayerInvisibleWall : NetworkBehaviour
{
    private MeshRenderer _invisbleWallMeshRenderer;

    private BoxCollider _invisibleWallBoxCollider;
    [SerializeField] private RG_StartButton _startButtonScript;

    [SerializeField] private GameObject _startButtonPrefab;

   // [SerializeField] private GameObject _clientCaution;

    void Start()
    {

            _invisbleWallMeshRenderer = GetComponent<MeshRenderer>();

            if ( _invisbleWallMeshRenderer != null)
            {
                _invisbleWallMeshRenderer.enabled = false;

                
            }
            

    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if(IsServer)
        {
            _startButtonScript = GameObject.FindObjectOfType<RG_StartButton>();
            _invisibleWallBoxCollider = GetComponent<BoxCollider>();
            if(_invisibleWallBoxCollider != null)
            {
                _invisibleWallBoxCollider.enabled = true;
            }
        }
    }

    private void Update()
    {
        if(_startButtonScript != null && IsHost)
        {
                bool _onPressed = _startButtonScript._pressed.Value;
                // Debug.Log("Wall down " + _onPressed);
                if(_onPressed == true)
                {
                    OnButtonPressed();
                }
                
        }

        // else{
        //     StartCoroutine(OnClientPressButton());
        // }
    }

    // IEnumerator OnClientPressButton()
    // {
    //     _clientCaution.gameObject.SetActive(true);
    //     yield return new WaitForSeconds (3.0f);
    //     _clientCaution.gameObject.SetActive(false);
    // }
    private void OnButtonPressed()
    {
        DestroyButtonClientRpc();
    }

    [ClientRpc]
    public void DestroyButtonClientRpc()
    {
        _invisibleWallBoxCollider = GetComponent<BoxCollider>();
        if(_invisibleWallBoxCollider != null)
        {
            _invisibleWallBoxCollider.enabled = false;
        }
    }

}
