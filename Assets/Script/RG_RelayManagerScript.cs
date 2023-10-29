using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Netcode;
using System.Threading.Tasks;
using TMPro;


public class RG_RelayManagerScript : Singleton<RG_RelayManagerScript>
{

    [SerializeField] private TMP_Text _codeText;

    public UnityTransport Transport => 
    NetworkManager.Singleton.GetComponent<UnityTransport>();
    public bool _isRelayEnabled => 
    Transport != null && Transport.Protocol == UnityTransport.ProtocolType.RelayUnityTransport;
    private async void Start(){
        InitializationOptions options = new InitializationOptions();
#if UNITY_EDITOR
        //options.SetProfile(ClonesManager.IsClone() ? ClonesManager.GetArgument() : "Primary");
#endif
        await UnityServices.InitializeAsync(options);
        
        if(!AuthenticationService.Instance.IsSignedIn){
            AuthenticationService.Instance.SignedIn += () =>{
                Debug.Log("Signed in" + AuthenticationService.Instance.PlayerId);
            };
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        }
    }

    
    public async Task CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            _codeText.text = joinCode;
            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            // NetworkManager.Singleton.StartHost();
            Debug.Log("Join Relay code = " + joinCode);
        } catch (RelayServiceException e) {
             Debug.Log(e); 
        }
    }

    
    public async Task JoinRelay(string joinCode)
    {
        try
        {
            Debug.Log("Joining Relay with " + joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            // NetworkManager.Singleton.StartClient();
        } catch (RelayServiceException e) { Debug.Log(e); }
    }
}
