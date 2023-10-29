using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class RG_DisableCharacterPrefab : NetworkBehaviour
{
    private CharacterController _characterController;
    
    private void Onenable(){
        if(_characterController == null){
            _characterController = GetComponentInChildren<CharacterController>();
        }
        if(_characterController != null){
            _characterController.enabled = true;
        }

    }

    public override void OnNetworkSpawn()
    {
        if(_characterController == null){
            _characterController = GetComponentInChildren<CharacterController>();
        }
        if(_characterController != null){
            _characterController.enabled = true;
        }
        base.OnNetworkSpawn();
    }
}
