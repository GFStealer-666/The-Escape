// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Unity.Netcode;

// public class RG_EnemySpawner : NetworkBehaviour
// {
//     [SerializeField] private GameObject _enemyPrefab;

//     [SerializeField] private List<Vector3> _enemySpawnLocationList = new List<Vector3>();

//     [SerializeField] private bool  _startSpawning = false;

//     [SerializeField] private RG_StartButton _startButtonScript;

//     [SerializeField] private int _spawnCount  = 1;
    
//     private int _randomNum;
//     void Start()
//     {

//         _enemySpawnLocationList.Clear();
//         _startButtonScript = GameObject.FindObjectOfType<RG_StartButton>();
            

//         Vector3 newPos = new Vector3(26.2f,4.6f,-1);
//         Vector3 newPos1 = new Vector3(26,5,-6);
//         Vector3 newPos2 = new Vector3(30,4.7f,2);
//         Vector3 newPos3 = new Vector3(9.4f,4.8f,-9.8f);
//         Vector3 newPos4 = new Vector3(14.4f,4.6f,-11.36f);
//         Vector3 newPos5 = new Vector3(0.47f,5,-10.33f);
//         Vector3 newPos6 = new Vector3(-14,4.7f,-10.63f);
//         Vector3 newPos7 = new Vector3(-26.45f,4.6f,2.7f);
//         Vector3 newPos8 = new Vector3(-14.65f,0.96f,-8.30f);
//         Vector3 newPos9 = new Vector3(-16,1.44f,4.07f);
//         Vector3 newPos10 = new Vector3(-27,1.05f,-11.10f);
//         Vector3 newPos11 = new Vector3(-7.87f,0.56f,-7.62f);
//         Vector3 newPos12 = new Vector3(7.75f,1.01f,-11.79f);
//         Vector3 newPos13 = new Vector3(26.27f,0.88f,-11.63f);
//         Vector3 newPos14 = new Vector3(29,1.16f,-5.95f);
//         _enemySpawnLocationList.Add(newPos);
//         _enemySpawnLocationList.Add(newPos1);
//         _enemySpawnLocationList.Add(newPos2);
//         _enemySpawnLocationList.Add(newPos3);
//         _enemySpawnLocationList.Add(newPos4);
//         _enemySpawnLocationList.Add(newPos5);
//         _enemySpawnLocationList.Add(newPos6);
//         _enemySpawnLocationList.Add(newPos7);
//         _enemySpawnLocationList.Add(newPos8);
//         _enemySpawnLocationList.Add(newPos9);
//         _enemySpawnLocationList.Add(newPos10);
//         _enemySpawnLocationList.Add(newPos11);
//         _enemySpawnLocationList.Add(newPos12);
//         _enemySpawnLocationList.Add(newPos13);
//         _enemySpawnLocationList.Add(newPos14);

//         Debug.Log(_enemySpawnLocationList[0]);
//         Debug.Log(_enemySpawnLocationList[10]);

//     }

//     // Update is called once per frame
//     void Update()
//     {
//         if(!IsServer) return;
//         _startSpawning = _startButtonScript._pressed.Value;
//         _startButtonScript._pressed.Value = false;
//         if(_startSpawning == true){
//             RandomSpawnEnemyPositionClientRpc(_spawnCount);
//             _startSpawning = false;
//         }

//     }

//     [ClientRpc]
//     void RandomSpawnEnemyPositionClientRpc(int _count)
//     {
//         if(!IsHost) return;
        
//         _randomNum = Random.Range(1,_enemySpawnLocationList.Count);   
//         Vector3 spawnPos = _enemySpawnLocationList[_randomNum];
//         Quaternion spawnRot = Quaternion.Euler(0, -90, 0);
//         GameObject enemy = Instantiate(_enemyPrefab, spawnPos , spawnRot);

//         enemy.GetComponent<RG_Enemy>()._enemySpawner = this;
//         enemy.GetComponent<NetworkObject>().Spawn(true);
//     }
    
// }
  
  

