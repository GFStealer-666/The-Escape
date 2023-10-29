using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using TMPro;

public class MainPlayerMovement : NetworkBehaviour{

    public float speed = 5.0f;
    public float rotationSpeed =10.0f;
    Rigidbody rb;

    private LoginManagerScript loginManager;

    public TMP_Text namePrefab;
    private TMP_Text nameLabel;

    [SerializeField] private List<string> userName =  new List<string>();

    private NetworkVariable<int> posX = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone , NetworkVariableWritePermission.Owner);

    
    public NetworkVariable<NetworkString> playerNameA = new NetworkVariable<NetworkString>
    (new NetworkString {info = "Player A"},NetworkVariableReadPermission.Everyone , NetworkVariableWritePermission.Owner);

    public NetworkVariable<NetworkString> playerNameB = new NetworkVariable<NetworkString>
    (new NetworkString {info = "Player B"},NetworkVariableReadPermission.Everyone , NetworkVariableWritePermission.Owner);
    
      public struct NetworkString : INetworkSerializable
    {
        public FixedString32Bytes info;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref info);
        }
        public override string ToString()
        {
            return info.ToString();
        }
        public static implicit operator NetworkString(string v) =>
            new NetworkString() { info = new FixedString32Bytes(v)};
    }
    
    public override void OnNetworkSpawn()
    {
        GameObject canvas = GameObject.FindWithTag("MainCanvas");
        nameLabel = Instantiate(namePrefab,Vector3.zero , Quaternion.identity) as TMP_Text;
        nameLabel.transform.SetParent(canvas.transform);
        
        // posX.OnValueChanged += (int previousValue , int newValue) =>{
        //     Debug.Log("OwnerID = " + OwnerClientId + " post x = " + posX.Value);
        // };

        // playerNameA.OnValueChanged += (NetworkString previousData, NetworkString newValue) =>
        // {
        //     Debug.Log("Owner ID = " + OwnerClientId + " : new data = " + newValue.info);
        // };

        // playerNameB.OnValueChanged += (NetworkString previousData, NetworkString newValue) =>
        // {
        //     Debug.Log("Owner ID = " + OwnerClientId + " : new data = " + newValue.info);
        // };

        // if(IsServer){
        //     playerNameA.Value = new NetworkString() { info = new FixedString32Bytes("Player 1")};

        //     playerNameB.Value = new NetworkString() { info = new FixedString32Bytes("Player 2")};
        // }
        if(IsOwner){
            loginManager = GameObject.FindObjectOfType<LoginManagerScript>();
            if(loginManager != null){
                string username = loginManager.userNameInputField.text;
                if(IsOwnedByServer){ playerNameA.Value = username;}
                else {playerNameB.Value = username;}
            }
        }

    }
    public void Start(){
        rb = this.GetComponent<Rigidbody>();
        loginManager = GameObject.FindObjectOfType<LoginManagerScript>();
        userName = loginManager.userUsername;
        
    }
    // private void update(){
        
    // }

    public override void OnDestroy()
    {
        if(nameLabel != null)
            Destroy(nameLabel.gameObject);
        
        base.OnDestroy();
    }

    private void Update(){
       
        Vector3 nameLabelPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0,2.5f , 0));
        nameLabel.text = gameObject.name;
        nameLabel.transform.position = nameLabelPos;

        if(IsOwner){
             posX.Value = (int)System.Math.Ceiling(transform.position.x);
        }

        updataPlayerInfo();
    }
    private void FixedUpdate() 
    {

        if(IsOwner){
           

            float translation = Input.GetAxis("Vertical") * speed;
            translation *= Time.deltaTime;
            rb.MovePosition(rb.position + this.transform.forward * translation);

            float rotation = Input.GetAxis("Horizontal");
            if(rotation != 0){
                rotation *= rotationSpeed;
                Quaternion turn = Quaternion.Euler(0f , rotation , 0f);
                rb.MoveRotation(rb.rotation * turn);
            }
            else{
                rb.angularVelocity = Vector3.zero;
            }
        }
       
    }

    private void updataPlayerInfo(){

        if (IsOwnedByServer)
        {
            nameLabel.text = playerNameA.Value.ToString();
        }
        else
        {
            nameLabel.text = playerNameB.Value.ToString();
        }
    }

    private void OnEnable() {
        if (nameLabel != null){
            nameLabel.enabled = true;
        }
    }

     private void OnDisable() {
        if (nameLabel != null){
            nameLabel.enabled = false;
        }
}
}
