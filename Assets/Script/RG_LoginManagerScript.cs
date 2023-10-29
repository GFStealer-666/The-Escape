using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay; 
using UnityEngine.InputSystem;


public class RG_LoginManagerScript : MonoBehaviour
{   
    [SerializeField] private GameObject _loginPanel;

    [SerializeField] private GameObject _leaveButton;


    [SerializeField] public TMP_InputField _userNameInputFromCreatePanel;
    [SerializeField] public TMP_InputField _userNameInputFromCodePanel;

    [SerializeField] public  TMP_InputField _joinCodeInput;
    
    public string _joinCode;

    private CharacterController _characterController;


    [SerializeField] private BoxCollider _playerSpawner;
    private int _spawnPoint = 5;
    public List<string> userUsername = new List<string>();
    private bool firstClient = true;

    public async void Client()
    {
        string userName = "";
        userName = _userNameInputFromCodePanel.GetComponent<TMP_InputField>().text;

        _joinCode = _joinCodeInput.GetComponent<TMP_InputField>().text;
        Debug.Log(_joinCode);
        if(RG_RelayManagerScript.Instance._isRelayEnabled && !string.IsNullOrEmpty(_joinCode)){

            _joinCode = _joinCode.Substring(0, 6);
            _joinCode = _joinCode.ToUpper();
            Debug.Log("Before joining client");
            await RG_RelayManagerScript.Instance.JoinRelay(_joinCode);
            Debug.Log("After joining client");

            // await RG_LobbyManagerScript.instance.JoinLobbyByCode(_joinCode);
        }
        
        Debug.Log(userName);
        // string passWord = passwordInputField.GetComponent<TMP_InputField>().text;
        // if(passWord == gamePassword){
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes(userName);
        NetworkManager.Singleton.StartClient();
        Debug.Log("Start Client = " + userName);
        
    }
    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
        SetUIVisible(false);
    }

    private void SetUIVisible(bool _isUserLogin)
    {
        // Debug.Log(_isUserLogin);
        if(_isUserLogin == true)
        {
            if(_loginPanel && _leaveButton != null){
                _loginPanel.SetActive(false);
                _leaveButton.SetActive(true);
            }
            
        }
        else{
            if(_loginPanel && _leaveButton != null){
                _loginPanel.SetActive(true);
                _leaveButton.SetActive(false);

            }
        }
    }

    private void HandleServerStarted()
    {

    }
    private void HandleClientConnected(ulong clientId){
        Debug.Log("clientId = "+ clientId);
        if(clientId == NetworkManager.Singleton.LocalClientId){
            SetUIVisible(true);

        }
    }
    private void HandleClientDisconnect(ulong clientId){
        Debug.Log("HandleClientDisconnect clientId = " + clientId);

        if(clientId == 0){
            Leave();
        }
    }

    public void Leave(){
        if(NetworkManager.Singleton.IsHost){
            NetworkManager.Singleton.Shutdown();
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;

        }
        else if(NetworkManager.Singleton.IsClient){
            NetworkManager.Singleton.Shutdown();
        }
        SetUIVisible(false);
    }

    public async void Host()
    {
        if(RG_RelayManagerScript.Instance._isRelayEnabled){
            await RG_RelayManagerScript.Instance.CreateRelay();
            Debug.Log("start host");
        }
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
        NetworkManager.Singleton.StartHost();
    }

    // Update is called once per fram
    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        Debug.Log("approval check");
        var clientId = request.ClientNetworkId;

        var connectionData = request.Payload;
        int byteLength = connectionData.Length;

        bool _isApproved = true;
        // int _approvalMessage = 0;
        if(byteLength > 0){
            string clientData = System.Text.Encoding.ASCII.GetString(connectionData,0,byteLength);
            string hostdata = _userNameInputFromCreatePanel.GetComponent<TMP_InputField>().text;
            _isApproved = approveConneciton(clientData , hostdata);
        }
        response.Approved = _isApproved;
        response.CreatePlayerObject = true;
        response.PlayerPrefabHash = null;
        setSpawnLocation(clientId , response);
        response.Pending = false;

    } 

     public bool approveConneciton(string clientdata , string hostdata){
        bool _isApprove;
        // int _errorMessage = 0;
        if(firstClient == true){        
            userUsername.Add(clientdata);
            firstClient = false;
        }
        Debug.Log("Host Data = " + hostdata);
        if(System.String.Equals(clientdata.Trim(),hostdata.Trim()) == true){
            _isApprove = false;
            // _errorMessage = 2;
        }
        else{
            _isApprove = true;
        }        
        if(userUsername.Count > 1){
            foreach (string checkedName in userUsername){
                if(clientdata == checkedName){
                    _isApprove = false;
                }
            }
        }
        if(_isApprove == true){
            userUsername.Add(clientdata);
        }  
        return (_isApprove);

    }

    private void setSpawnLocation(ulong clientId , NetworkManager.ConnectionApprovalResponse response){
        
        Debug.Log("SetspawnLocation");
     
        Vector3 spawnPos= Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;

        // Vector3[] spawnPointArray = new []{ new Vector3(-1.6f,0.02f,-12), new Vector3(-1.6f,0.02f,-10) , new Vector3(-1.6f,0.02f,-8) , 
        // new Vector3(1.6f,0.02f,-12), new Vector3(1.6f,0.02f,-10) , new Vector3(1.6f,0.02f,-8) ,new Vector3(1.49f,0.02f,-8.65f), new Vector3(1.39f,0.02f,-10)};
        Vector3[] spawnPointArray = new []{ new Vector3(-1.6f,0.0f,-12), new Vector3(1.6f,0.02f,-8) , new Vector3(1.6f,0.02f,-12) , new Vector3(-1.6f,0.0f,-8)};
        int _randnum = Random.Range(0,spawnPointArray.Length);
        while(_randnum == _spawnPoint)
        {
            _randnum = Random.Range(0,spawnPointArray.Length);
        }
        _spawnPoint = _randnum;
        spawnPos = (spawnPointArray[_spawnPoint]);

        // float _randomSpawnPosXMax = _playerSpawner.bounds.max.x;
        // float _randomSpawnPosXMin = _playerSpawner.bounds.min.x;
        // float _randomSpawnPosZMax = _playerSpawner.bounds.max.z;
        // float _randomSpawnPosZMin = _playerSpawner.bounds.min.z;


        // spawnPos = new Vector3(Random.Range(_randomSpawnPosXMin,_randomSpawnPosXMax),0,
        // Random.Range(_randomSpawnPosZMin,_randomSpawnPosZMax));
        spawnRot = Quaternion.Euler(0, 0, 0); // host position

        response.Position = spawnPos;
        response.Rotation = spawnRot;

        Debug.Log(clientId + "Pos : " + spawnPos  + "ROt :  "+ spawnRot);
    }
}
