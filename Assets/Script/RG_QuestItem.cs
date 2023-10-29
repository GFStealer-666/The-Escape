using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class RG_QuestItem : NetworkBehaviour
{
    public RG_QuestObjectSpawner _questObjectSpawner;

    private RG_ScoreManager _scoreManagerScript;

    public override void OnNetworkSpawn()
    {
        _scoreManagerScript = FindObjectOfType<RG_ScoreManager>();
    }
    
    private void OnCollisionEnter(Collision collision) 
    {
        if(!IsOwner) return;

        if(collision.gameObject.tag == "Player"){
            ulong _networkObjectId = GetComponent<NetworkObject>().NetworkObjectId;
            _questObjectSpawner.DestroySpawnedQuestObjectServerRpc(_networkObjectId);
            _scoreManagerScript.IncreaseTheScore();
            Debug.Log("Colliding : " + _networkObjectId);
        }
    }
}
