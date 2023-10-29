using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using TMPro;

public class RG_NameTag : NetworkBehaviour{


    private RG_LoginManagerScript _loginManagerScript;

    private RG_QuickJoinLobbyScript _quickJoinLobbyScript;

    public TMP_Text _namePrefab;
    private TMP_Text _nameLabel;
    private Vector3 _offset = new Vector3(0,2,0);
    [SerializeField] private List<string> _userName =  new List<string>();


    
    public NetworkVariable<NetworkString> _playerNameA = new NetworkVariable<NetworkString>
    (new NetworkString {info = "Player A"},NetworkVariableReadPermission.Everyone , NetworkVariableWritePermission.Owner);

    public NetworkVariable<NetworkString> _playerNameB = new NetworkVariable<NetworkString>
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
        _nameLabel = Instantiate(_namePrefab,Vector3.zero + _offset, Quaternion.identity) as TMP_Text;
        _nameLabel.transform.SetParent(canvas.transform);

        if(IsOwner){
            _loginManagerScript = GameObject.FindObjectOfType<RG_LoginManagerScript>();
            _quickJoinLobbyScript = GameObject.FindObjectOfType<RG_QuickJoinLobbyScript>();
            string createPanelUsername = _loginManagerScript._userNameInputFromCreatePanel.text;
            string quickJoinUsername = _quickJoinLobbyScript._playNameInput.text;
            string joinViaCodeUsername = _loginManagerScript._userNameInputFromCodePanel.text;
            if(_loginManagerScript != null ){
                string username = "";
                if(!string.IsNullOrEmpty(createPanelUsername)){
                    username = createPanelUsername;
                }
                else if(!string.IsNullOrEmpty(joinViaCodeUsername)){
                    username = joinViaCodeUsername;
                }
                
                if(IsOwnedByServer){ _playerNameA.Value = username;}
                else {_playerNameB.Value = username;}
            }
            else if(_quickJoinLobbyScript != null &&  (createPanelUsername == "" && joinViaCodeUsername == "")){
                string username = quickJoinUsername;
                if(IsOwnedByServer){ _playerNameA.Value = username;}
                else {_playerNameB.Value = username;}
            }
        }

    }
    public void Start(){

        
    }

    private void Update(){
       
        Vector3 nameLabelPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0,2.0f , 0));
        _nameLabel.text = gameObject.name;
        _nameLabel.transform.position = nameLabelPos;

        updataPlayerInfo();
    }

    

    private void updataPlayerInfo(){

        if (IsOwnedByServer)
        {
            _nameLabel.text = _playerNameA.Value.ToString();
        }
        else
        {
            _nameLabel.text = _playerNameB.Value.ToString();
        }
    }

    public override void OnDestroy()
    {
        if(_nameLabel != null)
            Destroy(_nameLabel.gameObject);
        
        base.OnDestroy();
    }

    private void OnEnable() {
        if (_nameLabel != null){
            _nameLabel.enabled = true;
        }
    }

     private void OnDisable() {
        if (_nameLabel != null){
            _nameLabel.enabled = false;
        }
    }


}




