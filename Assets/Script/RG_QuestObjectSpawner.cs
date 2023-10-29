using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class RG_QuestObjectSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject _clipboardPrefab;

    // [SerializeField] private List<Vector3> _objectSpawnLocationList = new List<Vector3>(){
    //     new Vector3(26.2f,4.6f,-1) , new Vector3(26,5,-6) , new Vector3(30,4.7f,2) , new Vector3(9.4f,4.8f,-9.8f) , new Vector3(14.4f,4.6f,-11.36f) ,
    //     new Vector3(0.47f,5,-10.33f) , new Vector3(-14,4.7f,-10.63f) , new Vector3(-26.45f,4.6f,2.7f) , new Vector3(-14.65f,0.96f,-8.30f) , new Vector3(-16,1.44f,4.07f) , new Vector3(-27,1.05f,-11.10f) , new Vector3(-7.87f,0.56f,-7.62f) ,
    //     new Vector3(7.75f,1.01f,-11.79f) , new Vector3(26.27f,0.88f,-11.63f) , new Vector3(29,1.16f,-5.95f)}
    // ;

    [SerializeField] private List<Vector3> _objectSpawnLocationList = new List<Vector3>();
    [SerializeField] private List<int> _objectSpawnLocationIndex = new List<int>();

    [SerializeField] private bool  _startSpawning = false;

    [SerializeField] private List<GameObject> _spawnedQuestObjectList = new List<GameObject>();

    [SerializeField] private RG_StartButton _startButtonScript;

    [SerializeField] private int _spawnCount  = 5;

    [SerializeField] private bool disableSpawnCounter = false;
    

    private int _randomNum;
    void Start()
    {

        _objectSpawnLocationList.Clear();
        _startButtonScript = GameObject.FindObjectOfType<RG_StartButton>();
            

        Vector3 newPos = new Vector3(26.2f,4.6f,-1);
        Vector3 newPos1 = new Vector3(26,5,-6);
        Vector3 newPos2 = new Vector3(30,4.7f,2);
        Vector3 newPos3 = new Vector3(9.4f,4.8f,-9.8f);
        Vector3 newPos4 = new Vector3(14.4f,4.6f,-11.36f);
        Vector3 newPos5 = new Vector3(0.47f,5,-10.33f);
        Vector3 newPos6 = new Vector3(-14,4.7f,-10.63f);
        Vector3 newPos7 = new Vector3(-26.45f,4.6f,2.7f);
        Vector3 newPos8 = new Vector3(-14.65f,0.96f,-8.30f);
        Vector3 newPos9 = new Vector3(-16,1.44f,4.07f);
        Vector3 newPos10 = new Vector3(-27,1.05f,-11.10f);
        Vector3 newPos11 = new Vector3(-7.87f,0.56f,-7.62f);
        Vector3 newPos12 = new Vector3(7.75f,1.01f,-11.79f);
        Vector3 newPos13 = new Vector3(26.27f,0.88f,-11.63f);
        Vector3 newPos14 = new Vector3(29,1.16f,-5.95f);
        _objectSpawnLocationList.Add(newPos);
        _objectSpawnLocationList.Add(newPos1);
        _objectSpawnLocationList.Add(newPos2);
        _objectSpawnLocationList.Add(newPos3);
        _objectSpawnLocationList.Add(newPos4);
        _objectSpawnLocationList.Add(newPos5);
        _objectSpawnLocationList.Add(newPos6);
        _objectSpawnLocationList.Add(newPos7);
        _objectSpawnLocationList.Add(newPos8);
        _objectSpawnLocationList.Add(newPos9);
        _objectSpawnLocationList.Add(newPos10);
        _objectSpawnLocationList.Add(newPos11);
        _objectSpawnLocationList.Add(newPos12);
        _objectSpawnLocationList.Add(newPos13);
        _objectSpawnLocationList.Add(newPos14);

        // Debug.Log(_objectSpawnLocationList[0]);
        // Debug.Log(_objectSpawnLocationList[10]);

    }

    // Update is called once per frame
    void Update()
    {
        if(!IsHost) return;
        
        if(disableSpawnCounter == false){
            _startSpawning = _startButtonScript._pressed.Value;
            if(_startSpawning == true){
                RandomSpawnQuestObjectPositionClientRpc(_spawnCount);
                _startSpawning = false;
                disableSpawnCounter = !disableSpawnCounter;
            }
            
        }
    }

    [ClientRpc]
    void RandomSpawnQuestObjectPositionClientRpc(int _count)
    {
        if(!IsHost) return;
        int _spawnedPointListIndex = _objectSpawnLocationList.Count + 1;
        bool _alreadyExist = false;
        for(int i = 0 ; i < _count ; i++){
            _randomNum = Random.Range(1,_objectSpawnLocationList.Count);
            if(_objectSpawnLocationIndex.Count != 0)
            {
                _alreadyExist = _objectSpawnLocationIndex.Contains(_randomNum);
                while(_alreadyExist == true)
                {
                    _randomNum = Random.Range(1,_objectSpawnLocationList.Count);
                    _alreadyExist = _objectSpawnLocationIndex.Contains(_randomNum);
                }
            }                   

            _objectSpawnLocationIndex.Add(_randomNum);
            _spawnedPointListIndex = _randomNum;
            Vector3 spawnPos = _objectSpawnLocationList[_spawnedPointListIndex];
            Quaternion spawnRot = Quaternion.Euler(0, -90, 0);
            GameObject clipboard = Instantiate(_clipboardPrefab, spawnPos , spawnRot);
            _spawnedQuestObjectList.Add(clipboard);
            clipboard.GetComponent<RG_QuestItem>()._questObjectSpawner = this;
            clipboard.GetComponent<NetworkObject>().Spawn(true);
        }

    }

    [ServerRpc (RequireOwnership = false)]
    public void DestroySpawnedQuestObjectServerRpc(ulong _networkObjectID)
    {
        GameObject _objectToDestroy = findQuestObjectFromNetworkID(_networkObjectID);
        if(_objectToDestroy == null) return;

        _objectToDestroy.GetComponent<NetworkObject>().Despawn();
        _spawnedQuestObjectList.Remove(_objectToDestroy);
        Destroy(_objectToDestroy);
    }

    private GameObject findQuestObjectFromNetworkID(ulong _networkObjectID){
        foreach (GameObject _clipboard in _spawnedQuestObjectList)
        {
            ulong _clipboardID = _clipboard.GetComponent<NetworkObject>().NetworkObjectId;
            if(_clipboardID == _networkObjectID){
                return _clipboard;
            }
        }
        return null;
    }
}
