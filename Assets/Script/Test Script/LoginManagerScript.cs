using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
public class LoginManagerScript : MonoBehaviour
{
    public TMP_InputField userNameInputField;
    public TMP_InputField passwordInputField;

    [SerializeField] private BoxCollider _playerSpawner;

    public GameObject loginPanel;
    public GameObject leaveButton;
    public GameObject scorePanel;
    public string gamePassword;
    public List<string> userUsername = new List<string>();

    [SerializeField] private GameObject playerSpawner;
    

    bool firstClient = true;
    private void Start(){
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
        SetUIVisible(false);
    }

    private void SetUIVisible(bool isUserLogin)
    {
        if (isUserLogin)
        {
            loginPanel.SetActive(false);
            leaveButton.SetActive(true);
            scorePanel.SetActive(true);
        }
        else
        {
            loginPanel.SetActive(true);
            leaveButton.SetActive(false);
            scorePanel.SetActive(false);
        }
    }

    private void HandleServerStarted(){
        // Debug.Log("HandleSeverStarted");
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
    // Start is called before the first frame update
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



    public void Host()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
        NetworkManager.Singleton.StartHost();
        // Debug.Log("Start Host");
    }

    // Update is called once per frame
    public void Client()
    {
        string userName = userNameInputField.GetComponent<TMP_InputField>().text;
        string passWord = passwordInputField.GetComponent<TMP_InputField>().text;
        if(passWord == gamePassword){
            NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes(userName);
            NetworkManager.Singleton.StartClient();
            Debug.Log("Start Client = " + userName);
            // userNameInputField.text = "";
            passwordInputField.text = "";

        }
        else{
            Debug.Log("wrong password");
            passwordInputField.text = "";
        }
        
    }

    public bool  approveConneciton(string clientdata , string hostdata){
        bool _isApprove;
        int _checkedCounter = 0;
        Debug.Log("Client Data = " + clientdata);
        if(firstClient == true){
            
            userUsername.Add(clientdata);
            Debug.Log("first player jump in ba bitch!");
            firstClient = false;
        }
        Debug.Log("Host Data = " + hostdata);
        _isApprove = System.String.Equals(clientdata.Trim(),hostdata.Trim()) ? false : true;
        
        if(userUsername.Count > 1){
            foreach (string checkedName in userUsername){
                if(clientdata == checkedName){
                    _checkedCounter = 1;
                    Debug.Log("Repeated username");
                }
            }
        }
        if(_checkedCounter == 1){
            _isApprove = false;

        }
        if(_isApprove == true){
            userUsername.Add(clientdata);
        }  
        return _isApprove;

    }
    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        
        // The client identifier to be authenticated
        var clientId = request.ClientNetworkId;
        // Additional connection data defined by user code
        var connectionData = request.Payload;
        int byteLength = connectionData.Length;
        // Debug.Log("byte length = " + byteLength);
        bool _isApproved = false;
        if(byteLength > 0){
            string clientData = System.Text.Encoding.ASCII.GetString(connectionData,0,byteLength);
            // for(int i  = 0  ; i < connectionData.Length ; i++){
            //     Debug.Log("connection data" + connectionData[i]); 
            //     Debug.Log("byte length " + byteLength);
            // }
            string hostdata = userNameInputField.GetComponent<TMP_InputField>().text;
            _isApproved = approveConneciton(clientData , hostdata);
        }
        
        // Your approval logic determines the following values
        response.Approved = _isApproved;
        response.CreatePlayerObject = true;

        // The Prefab hash value of the NetworkPrefab, if null the default NetworkManager player Prefab is used
        response.PlayerPrefabHash = null;

        // Position to spawn the player object (if null it uses default of Vector3.zero)
        response.Position = Vector3.zero;

        // Rotation to spawn the player object (if null it uses the default of Quaternion.identity)
        response.Rotation = Quaternion.identity;

        setSpawnLocation(clientId , response);

        Debug.Log(clientId);
    
        // If response.Approved is false, you can provide a message that explains the reason why via ConnectionApprovalResponse.Reason
        // On the client-side, NetworkManager.DisconnectReason will be populated with this message via DisconnectReasonMessage
        
        // If additional approval steps are needed, set this to true until the additional steps are complete
        // once it transitions from true to false the connection approval response will be processed.
        response.Pending = false;
        // Debug.Log("Host Approval Check : " + response.Pending);
    }   

    private void setSpawnLocation(ulong clientId , NetworkManager.ConnectionApprovalResponse response){
        Vector3 spawnPos= Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;
        Vector3[] spawnPointArray = new [] {new Vector3(0f,0f,0f) , new Vector3(2f,0f,0f) , new Vector3(4f,0f,0f)  , new Vector3(6f,0f,0f) , new Vector3(8f,0f,0f) };
        // int arrayIndex = 0;
        // bool positionEmpty = false;
        // while(positionEmpty == false){
        //     arrayIndex = Random.Range(1,spawnPointArray.Length);
        //     positionEmpty = checkEmptySpawnPosition(spawnPointArray[arrayIndex]);
        //     if(positionEmpty == true){
        //         break;
        //     }
        // }
        
        // if(clientId == NetworkManager.Singleton.LocalClientId){
        //     spawnPos = spawnPointArray[0];
        //     spawnRot = Quaternion.Euler(0f , 135f , 0f);
        // }
        // else{
        //     spawnPos = spawnPointArray[arrayIndex];
        //     Debug.Log("spawnpoint : " + arrayIndex);
        //     spawnRot = Quaternion.Euler(0f , 135f , 0f);
        // }
        // else{
        //     switch(NetworkManager.Singc vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv
        //             spawnPos = new Vector3(0f,0f,0f);
        //             spawnRot = Quaternion.Euler(0f , 225f , 0f);
        //             break;
        //         case 2 : 
        //             spawnPos = new Vector3(2f,0f,0f);
        //             spawnRot = Quaternion.Euler(0f , 225f , 0f);
        //             break;
        //     }

        // }
        int randnum = Random.Range(0,spawnPointArray.Length);

        // float _randomSpawnPosXMax = _playerSpawner.bounds.max.x;
        // float _randomSpawnPosXMin = _playerSpawner.bounds.min.x;
        // float _randomSpawnPosZMax = _playerSpawner.bounds.max.z;
        // float _randomSpawnPosZMin = _playerSpawner.bounds.min.z;


        // spawnPos = new Vector3(Random.Range(_randomSpawnPosXMin,_randomSpawnPosXMax),0,
        // Random.Range(_randomSpawnPosZMin,_randomSpawnPosZMax));
        // spawnRot = Quaternion.Euler(0, -90, 0); // host position
        // if (clientId == NetworkManager.Singleton.LocalClientId)
        // {
        //     spawnPos = new Vector3(0, 0, 0); 
        // }
        // else{
        //     float randomPos = Random.Range(1,5);
        //     while(randomPos == spawnPoint){
        //         randomPos = Random.Range(1,5);
        //     }
        //     spawnPoint = randomPos;
        //     spawnPos = new Vector3(spawnPoint , 0 , 0);
        // }

        spawnPos = spawnPointArray[randnum];
        response.Position = spawnPos;
        response.Rotation = spawnRot;
        Debug.Log("CD : " +clientId);
        Debug.Log("PT : " + response.Position);
        Debug.Log("RT : " + response.Rotation);
    }

    // private bool checkEmptySpawnPosition(Vector3 spawnPoint){
    //     Collider[] intersecting = Physics.OverlapSphere(spawnPoint,0.01f);
    //     if(intersecting.Length == 0){
    //         return false;
    //     }
    //     else{
    //         return true;
    //     }
    // }

    // private bool _isApprovedConnection = true;
    // // [Command("set-approve")]
    // // public bool set_isApproveConnection(){
    // //     _isApprovedConnection = !_isApprovedConnection;
    // //     return _isApprovedConnection;
    // // }

    
}
