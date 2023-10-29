using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class RG_Door : NetworkBehaviour ,  IInteractable
{

    private bool _doorOpen = false;
    private Animator _door;

    [SerializeField] private string _prompt;
    public string InteractionPrompt => _prompt;
    public override void OnNetworkSpawn(){
        _door = GetComponent<Animator>();
    }
    public bool Interact(RG_Interactor interactor){
        DoorController();
        return true;
    }

    private void DoorController(){
        
        if(_doorOpen == false){
            OpenDoorServerRpc();
            _prompt = "E to Open";
        }
        else if(_doorOpen == true){
            CloseDoorServerRpc();
            _prompt = "E to Close";
        }
    }

    [ServerRpc (RequireOwnership = false)]
    public void OpenDoorServerRpc(){
        _door.SetBool("DoorOpen" , _doorOpen);
        _doorOpen = !_doorOpen;
    }

    [ServerRpc (RequireOwnership = false)]
    public void CloseDoorServerRpc(){
        _door.SetBool("DoorOpen" , _doorOpen);
        _doorOpen = !_doorOpen;
    }
}
