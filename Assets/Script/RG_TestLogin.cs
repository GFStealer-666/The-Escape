using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
public class RG_TestLogin : NetworkBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private Button _serverBtn;
    [SerializeField] private Button _hostBtn;
    [SerializeField] private Button _clientBtn;

    private void Awake(){
        _serverBtn.onClick.AddListener(call: () => NetworkManager.Singleton.StartServer());
        _hostBtn.onClick.AddListener(call: () => NetworkManager.Singleton.StartHost());
        _clientBtn.onClick.AddListener(call: () => NetworkManager.Singleton.StartClient());
    }
}
