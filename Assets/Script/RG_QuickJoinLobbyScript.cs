using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Netcode;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using TMPro;

public class RG_QuickJoinLobbyScript : MonoBehaviour
{
    public TMP_InputField _playNameInput;
    public GameObject _startButton;
    public GameObject _matchMakingPanel;
    private string _playerName;
    string _lobbyName = "MyLobby";
    private Lobby _joinedLobby;
    UnityTransport transport;

    [SerializeField] private TMP_Text _codeText;

    [SerializeField] private GameObject _joinCodePanel;

    public string _joinCode;


    [SerializeField] private BoxCollider _playerSpawner;
    private int _spawnPoint = 5;

    public List<string> userUsername = new List<string>();

    private bool firstClient = true;
    public async void StartGame()
    {
        _startButton.SetActive(false);
        _matchMakingPanel.SetActive(false);
        _playerName = _playNameInput.GetComponent<TMP_InputField>().text;
        //joinedLobby = await CreateLobby();
        //_joinCodeInput.GetComponent<TMP_InputField>().text.ToUpper();
        _joinedLobby = await JoinLobby() ?? await CreateLobby();
        if (_joinedLobby == null)
        {
            _startButton.SetActive(true);
            _matchMakingPanel.SetActive(true);
        }
    }

    // public async void JoinLobbyThroughCode(){
    //     string lobbyID = _joinCodeInput.GetComponent<TMP_InputField>().text;
    //     Debug.Log("Joincode " + lobbyID);
    //     await JoinLobbyViaCode (lobbyID);
    // }

    

    private async Task<Lobby> CreateLobby()
    {
        try
        {
            const int maxPlayer = 2;
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayer);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            _joinCodePanel.SetActive(true);
            _codeText.text = joinCode;
            Debug.Log("Logging and opening");


            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                
                IsPrivate = false,
                Player = new Player
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        {"PlayerName",
                            new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,_playerName)}
                    }
                },
                Data = new Dictionary<string, DataObject>
                {
                    {"JoinCodeKey", new DataObject(DataObject.VisibilityOptions.Public, joinCode) }
                }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(_lobbyName, maxPlayer, createLobbyOptions);
            StartCoroutine(HeartBeatLobbyCoroutine(lobby.Id, 15));
            Debug.Log("Created Lobby : " + lobby.Name + " , " + lobby.MaxPlayers + " , " + lobby.Id + " , " + lobby.LobbyCode);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();

            RG_LobbyManagerScript.Instance.PrintPlayer(lobby);
            return lobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return null;
        }
    }

    private static IEnumerator HeartBeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        var delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }

    private async Task<Lobby> JoinLobby()
    {
        try
        {
                Lobby lobby = await FindRandomLobby();
                if (lobby == null) return null;

                if (lobby.Data["JoinCodeKey"].Value != null)
                {
                    string joinCode = lobby.Data["JoinCodeKey"].Value;
                    Debug.Log("joincode = " + joinCode);
                    if(joinCode == null) return null;
                    joinCode = joinCode.Substring(0, 6);
                    JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

                    RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
                    NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
                    NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

                    NetworkManager.Singleton.StartClient();
                    return lobby;
                }
                return null;
            
            // else{
            //         Debug.Log("join via code");
            //         joinCode = joinCode.Substring(0, 6);
            //         Debug.Log(joinCode);
            //         JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            //         RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            //         NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
            //         NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            //         NetworkManager.Singleton.StartClient();
                
            //     }
            
            
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            Debug.Log("No lobby found");
            return null;
        }
    }

    private async Task JoinLobbyViaCode(string joincode){
        try{
            if(joincode != null){
                joincode = joincode.Substring(0, 6);
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joincode);
                RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
                NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
                NetworkManager.Singleton.StartClient();      
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            Debug.Log("No lobby found");

        }
    }

    private async Task<Lobby> FindRandomLobby()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots,"1",QueryFilter.OpOptions.GE)
                }
            };
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            Debug.Log("Lobbies found: " + queryResponse.Results.Count);
            foreach (Lobby lobby in queryResponse.Results)
            {
                return lobby;
            }
            return null;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return null;
        }
    }

    private async Task<Lobby> FindLobbyThroughtCode(string lobbycode)
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots,"1",QueryFilter.OpOptions.GE)
                }
            };
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            Debug.Log("Lobbies found: " + queryResponse.Results.Count);
            foreach (Lobby lobby in queryResponse.Results)
            {
                if(lobby.Data["JoinCodeKey"].Value != null && lobby.Data["JoinCodeKey"].Value == lobbycode ){
                    return lobby;
                }
            }
            return null;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return null;
        }
    }


    // BOMB CODE

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        Debug.Log("approval check");
        var clientId = request.ClientNetworkId;

        var connectionData = request.Payload;
        int byteLength = connectionData.Length;

        bool _isApproved = true;
        // int _approvalMessage = 0;
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
