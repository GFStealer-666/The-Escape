using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class RG_Flashlight : NetworkBehaviour
{

    private bool _haveFlashLight = false; // not needed right now 

    [SerializeField] private GameObject _flashLightPrefab;
    [SerializeField] private AudioListener _flashLightSound;

    private ulong _flashlightNetworkID;
    private bool _openFlashLight = false;

    void Update(){
        if(!IsLocalPlayer) return ;
        if(Keyboard.current.fKey.wasPressedThisFrame){
            FlashLightControlServerRpc(_openFlashLight);
            _openFlashLight = !_openFlashLight;
        }
    }

    [ServerRpc]
    public void FlashLightControlServerRpc(bool _open){
        Debug.Log("Hello");
        if(_open == false){
            SpawnFlashLightServerRpc();
        }
        else if(_open == true){
            DespawnFlashLightServerRpc(_flashlightNetworkID);
        }
            
    }

    [ServerRpc]
    public void SpawnFlashLightServerRpc(){
        Debug.Log("Hello1");

        Vector3 _spawnPos = new Vector3(transform.position.x , transform.position.y+1f , transform.position.z+0.2f);
        Quaternion _spawnRot = transform.rotation;
        GameObject _spawnFlashLight = Instantiate(_flashLightPrefab , _spawnPos , _spawnRot);
        _spawnFlashLight.GetComponent<NetworkObject>().Spawn();
        _spawnFlashLight.GetComponent<NetworkObject>().TrySetParent(this.gameObject);
       _flashlightNetworkID = _spawnFlashLight.GetComponent<NetworkObject>().NetworkObjectId;
       
    }



    [ServerRpc]
    public void DespawnFlashLightServerRpc(ulong _flashlightNetworkID){
        Debug.Log("Hello2");
        NetworkObject _spawnedFlashLight = NetworkManager.SpawnManager.SpawnedObjects[_flashlightNetworkID];
        if(_spawnedFlashLight == null) return;
        _spawnedFlashLight.Despawn();
        Destroy(_spawnedFlashLight);
        
    }
}
