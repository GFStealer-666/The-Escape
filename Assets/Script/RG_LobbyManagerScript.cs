using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Threading.Tasks;

public class RG_LobbyManagerScript : Singleton<RG_LobbyManagerScript>
{
    private Lobby hostLobby;
    private Lobby joinedLobby;
    private string _playerName;
    private float lobbyUpdateTimer;
    private void Start()
    {
        _playerName = "pName" + Random.Range(1, 999);
        Debug.Log("Player name = " + _playerName);
    }

    private void Update()
    {
        HandleLobbyPollForUpdates();
    }

    private async void HandleLobbyPollForUpdates()
    {
        if (joinedLobby != null)
        {
            lobbyUpdateTimer -= Time.deltaTime;
            if (lobbyUpdateTimer < 0f)
            {
                float lobbyUpdateTimerMax = 2.2f;
                lobbyUpdateTimer = lobbyUpdateTimerMax;
                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                joinedLobby = lobby;
            }
        }
    }

    
    private async void CreateLobby(){
        try{
            string _lobbyName = "NewLobby";
            int _maxPlayer = 2;
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = new Player
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        {"PlayerName", new PlayerDataObject(
                            PlayerDataObject.VisibilityOptions.Member,_playerName)}
                    }
                },
                Data = new Dictionary<string, DataObject>{
                    {"GameMode" , new DataObject(DataObject.VisibilityOptions.Public , "2 Players Mode")}
                }
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(_lobbyName,_maxPlayer,options);
            hostLobby = lobby;
            joinedLobby = hostLobby;
            StartCoroutine(HeartBeatLobbyCoroutine(hostLobby.Id  , 15));
            Debug.Log("Lobby is created : " + lobby.Name + " : " + lobby.MaxPlayers + " : " +
                lobby.Id + " : " + lobby.LobbyCode);
            PrintPlayer(lobby);
        }
        catch (LobbyServiceException e){
            Debug.Log(e);
        }
    }

    private static IEnumerator HeartBeatLobbyCoroutine(string _lobbyID , float _waitTimeSeconds){
        var _delay = new WaitForSecondsRealtime(_waitTimeSeconds);
        while(true){
            Lobbies.Instance.SendHeartbeatPingAsync(_lobbyID);
            yield return _delay;
        }
    }



    private async void JoinLobby(){
        try{
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync();
            await Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id);
            Debug.Log(queryResponse.Results[0].Name + "," + queryResponse.Results[0].AvailableSlots);

        }
        catch (LobbyServiceException e){
            Debug.Log(e);
        }
    }

    private async void QuickJoinLobby()
    {
        try
        {
            Lobby lobby = await Lobbies.Instance.QuickJoinLobbyAsync();
            Debug.Log("Joined Lobby : " + lobby.Name + "," + lobby.AvailableSlots);
        }
        catch (LobbyServiceException e) { Debug.Log(e); }
    }


    private async Task JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions
            {
                Player = new Player
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        {"PlayerName", new PlayerDataObject(
                            PlayerDataObject.VisibilityOptions.Member,_playerName)}
                    }
                }
            };
            Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, options);
            joinedLobby = lobby;
            Debug.Log("Joined Lobby by code : " + lobbyCode);
            PrintPlayer(joinedLobby);
        }
        
        catch (LobbyServiceException e) { Debug.Log(e); }
    }

    private async void LeaveLobby()
    {
        try
        {
            string playerId = AuthenticationService.Instance.PlayerId;
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    private async void KickPlayer(byte _playerID)
    {
        try
        {
            string playerId = joinedLobby.Players[_playerID].Id;
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void MigrateLobbyHost()
    {
        try
        {
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {
                HostId = joinedLobby.Players[1].Id
            });
            joinedLobby = hostLobby;
            PrintPlayer(hostLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void DeleteLobby()
    {
        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void ListLobbies(){
        try{
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions{
                Count = 25,
                Filters = new List<QueryFilter>{
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots,"0",QueryFilter.OpOptions.GT)
                },
                Order = new List<QueryOrder>{
                    new QueryOrder(false,   QueryOrder.FieldOptions.Created)
                }

            };         
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            Debug.Log("Lobbies found " + queryResponse.Results.Count);
            foreach (Lobby lobby in queryResponse.Results){
            // Debug.Log(lobby.Name + "," + ((byte)lobby.MaxPlayers ) + "," + lobby.Data["GameMode"].Value); 
            }
        }
        catch (LobbyServiceException e){
            Debug.Log(e);
        }
    }


    private async void UpdateLobbyGameMode(string _gameMode){
        try{
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id , new UpdateLobbyOptions{
                Data = new Dictionary<string , DataObject>{
                    {"GameMode" , new DataObject(DataObject.VisibilityOptions.Public , _gameMode)}
                }
            });
            joinedLobby = hostLobby;
            PrintPlayer(hostLobby);
        }
        catch (LobbyServiceException e){
            Debug.Log(e);
        }
    }

    
    private async void UpdatePlayerName(string newPlayerName)
    {
        try
        {
            _playerName = newPlayerName;
            await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id,
                AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
                {
                    Data = new Dictionary<string, PlayerDataObject> {
                    {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,_playerName)}
                }
                });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    

    private void PrintPlayers(){
        PrintPlayer(joinedLobby);
    }

    
    public void PrintPlayer(Lobby lobby)
    {
        // Debug.Log("Player in Lobby : " + lobby.Name + " : " + lobby.Data["JoinCodeKey"].Value);
        foreach(Player player in lobby.Players)
        {
            Debug.Log("Players = " + player.Id + " : " + player.Data["PlayerName"].Value);
        } 

    }

}
