using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class RG_StartButton : NetworkBehaviour , IInteractable
{
    // Start is called before the first frame update


    [SerializeField] public NetworkVariable<bool> _pressed = new NetworkVariable<bool>(false ,
     NetworkVariableReadPermission.Everyone ,
     NetworkVariableWritePermission.Server);

    public delegate void PressChanged(bool pressed);

    public static event PressChanged OnStartButtonPress;

    [SerializeField] private string _prompt;
    public string InteractionPrompt => _prompt;
    public bool Interact(RG_Interactor interactor)
    {
        OnSwitchPressedServerRpc();
        return true;

    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _pressed.OnValueChanged += OnValueChanged;

        if(!IsHost)
        {
            GameObject[] _button = GameObject.FindGameObjectsWithTag("StartButton");
            Destroy(_button[0]);
            // Debug.Log(_button[0].ToString());
        }
    }

    private void OnValueChanged(bool notPressed , bool pressed)
    {
        // if(pressed)
        // {
        //     Debug.Log("Pressed : " + _pressed.Value);
        // }
    }

    [ServerRpc]
    public void OnSwitchPressedServerRpc()
    {
        _pressed.Value = true;
        OnStartButtonPress?.Invoke(_pressed.Value);
        if(IsHost){
            this.gameObject.SetActive(false);
        }
    }



    


}
